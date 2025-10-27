using BLLProject.Interface;
using DALProject.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BLLProject.Repository
{
    public class ServiceProviderRepository : Repository<ServiceProvider, int>, IServiceProviderRepository
    {
        public ServiceProviderRepository(string connectionString)
            : base(connectionString, "ServiceProviders")
        {
        }

        protected override SqlParameter[] GetCreateParameters(ServiceProvider entity)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Email", entity.Email),
                new SqlParameter("@Phone", (object)entity.Phone ?? DBNull.Value),
                new SqlParameter("@Address", (object)entity.Address ?? DBNull.Value)
            };
        }

        protected override SqlParameter[] GetUpdateParameters(ServiceProvider entity)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@Id", entity.Id),
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Email", entity.Email),
                new SqlParameter("@Phone", (object)entity.Phone ?? DBNull.Value),
                new SqlParameter("@Address", (object)entity.Address ?? DBNull.Value)
            };
        }

        public override async Task<int> CreateAsync(ServiceProvider entity)
        {
            return await ExecuteNonQueryStoredProcedureAsync("sp_CreateServiceProvider",
                GetCreateParameters(entity));
        }

        public override async Task<int> UpdateAsync(ServiceProvider entity)
        {
            return await ExecuteNonQueryStoredProcedureAsync("sp_UpdateServiceProvider",
                GetUpdateParameters(entity));
        }

        public override async Task<int> DeleteAsync(int id)
        {
            return await ExecuteNonQueryStoredProcedureAsync("sp_DeleteServiceProvider",
                new SqlParameter("@Id", id));
        }

        public async Task<ServiceProvider?> GetByEmailAsync(string email)
        {
            // Since we don't have a dedicated stored procedure for this, we'll use direct SQL
            await using var connection = await CreateConnectionAsync();
            await using var command = new SqlCommand("SELECT * FROM ServiceProviders WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable.Rows.Count > 0 ? MapDataRowToEntity(dataTable.Rows[0]) : null;
        }

        public async Task<IEnumerable<ServiceProvider>> SearchByNameAsync(string name)
        {
            await using var connection = await CreateConnectionAsync();
            await using var command = new SqlCommand("SELECT * FROM ServiceProviders WHERE Name LIKE @Name", connection);
            command.Parameters.AddWithValue("@Name", $"%{name}%");

            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return MapDataTableToList(dataTable);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var provider = await GetByEmailAsync(email);
            return provider != null;
        }
    }
}