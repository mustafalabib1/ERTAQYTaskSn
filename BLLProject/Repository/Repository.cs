using BLLProject.Interface;
using BLLProject.Repository;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BLLProject.Repository
{
    public abstract class Repository<T, TId> : IRepository<T, TId> where T : class, new()
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;

        protected Repository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        protected async Task<SqlConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected T MapDataRowToEntity(DataRow row)
        {
            var entity = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                {
                    var value = row[property.Name];
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (value.GetType() != propertyType)
                    {
                        value = Convert.ChangeType(value, propertyType);
                    }

                    property.SetValue(entity, value);
                }
            }

            return entity;
        }

        protected List<T> MapDataTableToList(DataTable dataTable)
        {
            var list = new List<T>();
            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(MapDataRowToEntity(row));
            }
            return list;
        }

        protected async Task<DataTable> ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] parameters)
        {
            await using var connection = await CreateConnectionAsync();
            await using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        protected async Task<int> ExecuteNonQueryStoredProcedureAsync(string procedureName, params SqlParameter[] parameters)
        {
            await using var connection = await CreateConnectionAsync();
            await using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return await command.ExecuteNonQueryAsync();
        }

        // Abstract methods for specific entity mapping
        protected abstract SqlParameter[] GetCreateParameters(T entity);
        protected abstract SqlParameter[] GetUpdateParameters(T entity);

        public virtual async Task<T?> GetByIdAsync(TId id)
        {
            var dataTable = await ExecuteStoredProcedureAsync($"sp_Get{_tableName}ById",
                new SqlParameter("@Id", id));

            return dataTable.Rows.Count > 0 ? MapDataRowToEntity(dataTable.Rows[0]) : null;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var dataTable = await ExecuteStoredProcedureAsync($"sp_GetAll{_tableName}");
            return MapDataTableToList(dataTable);
        }

        public virtual async Task<int> CreateAsync(T entity)
        {
            var parameters = GetCreateParameters(entity);
            var dataTable = await ExecuteStoredProcedureAsync($"sp_Create{_tableName}", parameters);
            if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["Id"] != DBNull.Value)
            {
                var id = Convert.ToInt32(dataTable.Rows[0]["Id"]);
                // Set the entity's Id property
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null && idProperty.CanWrite)
                {
                    idProperty.SetValue(entity, id);
                }
            }
            return 1;
        }
        public abstract Task<int> UpdateAsync(T entity);
        public abstract Task<int> DeleteAsync(TId id);
    }
}