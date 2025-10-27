using BLLProject.Interface;
using DALProject.Entities;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace BLLProject.Repository
{
    public class ProductRepository : Repository<Product, int>, IProductRepository
        {
            public ProductRepository(string connectionString)
                : base(connectionString, "Products")
            {
            }

            protected override SqlParameter[] GetCreateParameters(Product entity)
            {
                return new SqlParameter[]
                {
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Price", entity.Price),
                new SqlParameter("@ServiceProviderId", entity.ServiceProviderId),
                new SqlParameter("@CreationDate", entity.CreationDate)
                };
            }

            protected override SqlParameter[] GetUpdateParameters(Product entity)
            {
                return new SqlParameter[]
                {
                new SqlParameter("@Id", entity.Id),
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Price", entity.Price),
                new SqlParameter("@CreationDate", entity.CreationDate),
                new SqlParameter("@ServiceProviderId", entity.ServiceProviderId)
                };
            }

        public override async Task<int> CreateAsync(Product entity)
        {
            try
            {
                // Use ExecuteStoredProcedureAsync instead of ExecuteNonQueryStoredProcedureAsync
                // to get the returned ID
                var dataTable = await ExecuteStoredProcedureAsync("sp_CreateProduct",
                    GetCreateParameters(entity));

                if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["Id"] != DBNull.Value)
                {
                    var generatedId = Convert.ToInt32(dataTable.Rows[0]["Id"]);
                    entity.Id = generatedId; // Set the ID on the entity
                    return generatedId;
                }

                return 0; // Return 0 if no ID was returned
            }
            catch (Exception ex)
            {
                // Log error
                throw new InvalidOperationException("Error creating product and retrieving ID", ex);
            }
        }

        public override async Task<int> UpdateAsync(Product entity)
            {
                return await ExecuteNonQueryStoredProcedureAsync("sp_UpdateProduct",
                    GetUpdateParameters(entity));
            }

            public override async Task<int> DeleteAsync(int id)
            {
                return await ExecuteNonQueryStoredProcedureAsync("sp_DeleteProduct",
                    new SqlParameter("@Id", id));
            }

            public async Task<IEnumerable<Product>> GetProductsAbovePriceAsync(decimal minPrice)
            {
                var products = await GetFilteredProductsAsync(minPrice, null, null, null, null);
                return products;
            }

            public async Task<IEnumerable<Product>> GetProductsByDateRangeAsync(DateTime startDate, DateTime endDate)
            {
                var products = await GetFilteredProductsAsync(null, null, startDate, endDate, null);
                return products;
            }

            public async Task<bool> ServiceProviderExistsAsync(int serviceProviderId)
            {
                await using var connection = await CreateConnectionAsync();
                await using var command = new SqlCommand("SELECT COUNT(*) FROM ServiceProviders WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", serviceProviderId);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }

            public override async Task<IEnumerable<Product>> GetAllAsync()
            {
                await using var connection = await CreateConnectionAsync();
                await using var command = new SqlCommand(
                    @"SELECT p.*, 
                             sp.Id AS ServiceProvider_Id, 
                             sp.Name AS ServiceProvider_Name, 
                             sp.Email AS ServiceProvider_Email, 
                             sp.Phone AS ServiceProvider_Phone, 
                             sp.Address AS ServiceProvider_Address, 
                             sp.CreatedDate AS ServiceProvider_CreatedDate
                      FROM Products p
                      INNER JOIN ServiceProviders sp ON p.ServiceProviderId = sp.Id", connection);

                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                return MapDataTableToListWithServiceProvider(dataTable);
            }

            public override async Task<Product?> GetByIdAsync(int id)
            {
                await using var connection = await CreateConnectionAsync();
                await using var command = new SqlCommand(
                    @"SELECT p.*, 
                             sp.Id AS ServiceProvider_Id, 
                             sp.Name AS ServiceProvider_Name, 
                             sp.Email AS ServiceProvider_Email, 
                             sp.Phone AS ServiceProvider_Phone, 
                             sp.Address AS ServiceProvider_Address, 
                             sp.CreatedDate AS ServiceProvider_CreatedDate
                      FROM Products p
                      INNER JOIN ServiceProviders sp ON p.ServiceProviderId = sp.Id
                      WHERE p.Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable.Rows.Count > 0 ? MapDataRowToEntityWithServiceProvider(dataTable.Rows[0]) : null;
            }

            private Product MapDataRowToEntityWithServiceProvider(DataRow row)
            {
                var product = new Product();
                var productProperties = typeof(Product).GetProperties().Where(p => p.PropertyType != typeof(ServiceProvider) && p.Name != "ServiceProvider").ToArray();

                foreach (var property in productProperties)
                {
                    if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        var value = row[property.Name];
                        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                        if (value.GetType() != propertyType)
                        {
                            value = Convert.ChangeType(value, propertyType);
                        }

                        property.SetValue(product, value);
                    }
                }

                // Map ServiceProvider
                if (row.Table.Columns.Contains("ServiceProvider_Id"))
                {
                    var serviceProvider = new ServiceProvider();
                    serviceProvider.Id = Convert.ToInt32(row["ServiceProvider_Id"]);
                    serviceProvider.Name = row["ServiceProvider_Name"]?.ToString() ?? string.Empty;
                    serviceProvider.Email = row["ServiceProvider_Email"]?.ToString() ?? string.Empty;
                    serviceProvider.Phone = row["ServiceProvider_Phone"]?.ToString();
                    serviceProvider.Address = row["ServiceProvider_Address"]?.ToString();
                    serviceProvider.CreatedDate = Convert.ToDateTime(row["ServiceProvider_CreatedDate"]);
                    
                    product.ServiceProvider = serviceProvider;
                }

                return product;
            }

            private List<Product> MapDataTableToListWithServiceProvider(DataTable dataTable)
            {
                var list = new List<Product>();
                foreach (DataRow row in dataTable.Rows)
                {
                    list.Add(MapDataRowToEntityWithServiceProvider(row));
                }
                return list;
            }

            public async Task<IEnumerable<Product>> GetFilteredProductsAsync(decimal? minPrice, decimal? maxPrice, DateTime? fromDate, DateTime? toDate, int? serviceProviderId)
            {
                await using var connection = await CreateConnectionAsync();
                
                var query = @"SELECT p.*, 
                                     sp.Id AS ServiceProvider_Id, 
                                     sp.Name AS ServiceProvider_Name, 
                                     sp.Email AS ServiceProvider_Email, 
                                     sp.Phone AS ServiceProvider_Phone, 
                                     sp.Address AS ServiceProvider_Address, 
                                     sp.CreatedDate AS ServiceProvider_CreatedDate
                              FROM Products p
                              INNER JOIN ServiceProviders sp ON p.ServiceProviderId = sp.Id
                              WHERE 1=1";
                await using var command = new SqlCommand();

                if (minPrice.HasValue)
                {
                    query += " AND p.Price >= @MinPrice";
                    command.Parameters.AddWithValue("@MinPrice", minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query += " AND p.Price <= @MaxPrice";
                    command.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);
                }

                if (fromDate.HasValue)
                {
                    query += " AND p.CreationDate >= @FromDate";
                    command.Parameters.AddWithValue("@FromDate", fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query += " AND p.CreationDate <= @ToDate";
                    command.Parameters.AddWithValue("@ToDate", toDate.Value);
                }

                if (serviceProviderId.HasValue)
                {
                    query += " AND p.ServiceProviderId = @ServiceProviderId";
                    command.Parameters.AddWithValue("@ServiceProviderId", serviceProviderId.Value);
                }

                command.CommandText = query;
                command.Connection = connection;

                using var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                return MapDataTableToListWithServiceProvider(dataTable);
            }

            public async Task<IEnumerable<Product>> GetByServiceProviderIdAsync(int serviceProviderId)
            {
                var products = await GetFilteredProductsAsync(null, null, null, null, serviceProviderId);
                return products;
            }
        }
    }