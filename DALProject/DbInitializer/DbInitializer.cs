using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DALProject.DbInitializer
{
    public partial class DbInitializer : IDbInitializer
    {
        private readonly string _masterConnectionString;
        private readonly string _dbConnectionString;
        private readonly string _dbName;
        private readonly string _sqlFilePath;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(string connectionString, ILogger<DbInitializer> logger = null)
            : this(connectionString, "DbInitializer.sql", logger)
        {
        }

        public DbInitializer(string connectionString, string sqlFilePath, ILogger<DbInitializer> logger = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(sqlFilePath))
                throw new ArgumentException("SQL file path cannot be null or empty", nameof(sqlFilePath));

            var builder = new SqlConnectionStringBuilder(connectionString);

            if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
                throw new ArgumentException("Database name (Initial Catalog) must be specified in connection string");

            _dbName = builder.InitialCatalog;
            _dbConnectionString = builder.ConnectionString;
            _sqlFilePath = sqlFilePath;
            _logger = logger;

            // Connection string to connect to the master database
            builder.InitialCatalog = "master";
            _masterConnectionString = builder.ConnectionString;
        }

        public async Task InitializeAsync()
        {
            _logger?.LogInformation("Starting database initialization for database: {DatabaseName}", _dbName);

            try
            {
                if (!await DatabaseExistsAsync())
                {
                    _logger?.LogInformation("Database {DatabaseName} does not exist. Creating...", _dbName);
                    await CreateDatabaseAsync();
                    await CreateTablesAndStoredProceduresAsync();
                    _logger?.LogInformation("Database {DatabaseName} initialized successfully", _dbName);
                }
                else
                {
                    _logger?.LogInformation("Database {DatabaseName} already exists", _dbName);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Database initialization failed for database: {DatabaseName}", _dbName);
                throw new InvalidOperationException($"Database initialization failed for database '{_dbName}'.", ex);
            }
        }

        public async Task<bool> DatabaseExistsAsync()
        {
            try
            {
                await using var connection = new SqlConnection(_masterConnectionString);
                await connection.OpenAsync();

                const string query = """
                    SELECT COUNT(*) 
                    FROM sys.databases 
                    WHERE Name = @dbName AND state = 0
                    """;

                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@dbName", _dbName);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking if database exists: {DatabaseName}", _dbName);
                throw;
            }
        }

        private static string HidePasswordFromConnectionString(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                if (!string.IsNullOrEmpty(builder.Password))
                {
                    builder.Password = "***";
                }
                return builder.ConnectionString;
            }
            catch
            {
                return "Invalid connection string";
            }
        }

        private async Task CreateDatabaseAsync()
        {
            try
            {
                await using var connection = new SqlConnection(_masterConnectionString);
                await connection.OpenAsync();

                // Use parameterized query or proper quoting to prevent SQL injection
                var query = $"""
                    CREATE DATABASE {QuoteIdentifier(_dbName)};
                    ALTER DATABASE {QuoteIdentifier(_dbName)} SET READ_COMMITTED_SNAPSHOT ON;
                    """;

                await using var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating database: {DatabaseName}", _dbName);
                throw;
            }
        }

        private async Task CreateTablesAndStoredProceduresAsync()
        {
            if (!File.Exists(_sqlFilePath))
            {
                var errorMessage = $"Database script file not found: {Path.GetFullPath(_sqlFilePath)}";
                _logger?.LogError(errorMessage);
                throw new FileNotFoundException(errorMessage, _sqlFilePath);
            }

            _logger?.LogInformation("Executing SQL script: {SqlFilePath}", _sqlFilePath);

            try
            {
                var script = await File.ReadAllTextAsync(_sqlFilePath);
                var commandStrings = SplitSqlCommands(script);

                await using var connection = new SqlConnection(_dbConnectionString);
                await connection.OpenAsync();

                for (int i = 0; i < commandStrings.Count; i++)
                {
                    var commandString = commandStrings[i];
                    if (!string.IsNullOrWhiteSpace(commandString))
                    {
                        try
                        {
                            _logger?.LogDebug("Executing command {CommandIndex} of {TotalCommands}", i + 1, commandStrings.Count);

                            await using var command = new SqlCommand(commandString, connection);
                            await command.ExecuteNonQueryAsync();
                        }
                        catch (SqlException ex) when (ex.Number == 2714) // Object already exists
                        {
                            _logger?.LogWarning("SQL object already exists, skipping: {ErrorMessage}", ex.Message);
                            // Continue with next command
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error executing SQL command {CommandIndex}: {CommandPreview}",
                                i + 1, commandString.Length > 100 ? commandString.Substring(0, 100) + "..." : commandString);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error executing SQL script: {SqlFilePath}", _sqlFilePath);
                throw;
            }
        }

        private static List<string> SplitSqlCommands(string script)
        {
            // More robust SQL command splitting that handles:
            // - GO in different cases
            // - GO with semicolons
            // - Comments containing GO
            // - String literals containing GO

            var commands = new List<string>();
            var currentCommand = new StringBuilder();
            var inString = false;
            var inComment = false;
            var commentType = '\0'; // '/' for single-line, '*' for multi-line

            for (int i = 0; i < script.Length; i++)
            {
                var currentChar = script[i];
                var lookahead = i + 1 < script.Length ? script[i + 1] : '\0';

                // Handle string literals
                if (!inComment && currentChar == '\'')
                {
                    inString = !inString;
                    currentCommand.Append(currentChar);
                    continue;
                }

                // Handle comments
                if (!inString)
                {
                    if (!inComment && currentChar == '/' && lookahead == '*')
                    {
                        inComment = true;
                        commentType = '*';
                        currentCommand.Append(currentChar);
                        currentCommand.Append(lookahead);
                        i++; // Skip next character
                        continue;
                    }
                    else if (!inComment && currentChar == '-' && lookahead == '-')
                    {
                        inComment = true;
                        commentType = '/';
                        currentCommand.Append(currentChar);
                        currentCommand.Append(lookahead);
                        i++; // Skip next character
                        continue;
                    }
                    else if (inComment && commentType == '*' && currentChar == '*' && lookahead == '/')
                    {
                        inComment = false;
                        currentCommand.Append(currentChar);
                        currentCommand.Append(lookahead);
                        i++; // Skip next character
                        continue;
                    }
                    else if (inComment && commentType == '/' && currentChar == '\n')
                    {
                        inComment = false;
                    }
                }

                if (!inString && !inComment)
                {
                    // Check for GO command (case-insensitive)
                    if (i + 2 < script.Length &&
                        char.ToUpperInvariant(currentChar) == 'G' &&
                        char.ToUpperInvariant(lookahead) == 'O' &&
                        IsGoTerminator(script[i + 2]))
                    {
                        // Add current command to list
                        var command = currentCommand.ToString().Trim();
                        if (!string.IsNullOrEmpty(command))
                        {
                            commands.Add(command);
                        }
                        currentCommand.Clear();

                        // Skip the GO command
                        i += 2; // Skip "GO"
                        continue;
                    }
                }

                currentCommand.Append(currentChar);
            }

            // Add the final command
            var finalCommand = currentCommand.ToString().Trim();
            if (!string.IsNullOrEmpty(finalCommand))
            {
                commands.Add(finalCommand);
            }

            return commands;
        }

        private static bool IsGoTerminator(char c)
        {
            return char.IsWhiteSpace(c) || c == '\r' || c == '\n';
        }

        private static string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));

            return "[" + identifier.Replace("]", "]]") + "]";
        }
    }
}