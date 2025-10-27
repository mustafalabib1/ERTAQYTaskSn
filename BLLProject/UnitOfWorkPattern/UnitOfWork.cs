using BLLProject.Interface;
using BLLProject.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLProject.UnitOfWorkPattern
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private IServiceProviderRepository _serviceProviders;
        private IProductRepository _products;

        public UnitOfWork(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IServiceProviderRepository ServiceProviders
        {
            get
            {
                return _serviceProviders ??= new ServiceProviderRepository(_connectionString);
            }
        }

        public IProductRepository Products
        {
            get
            {
                return _products ??= new ProductRepository(_connectionString);
            }
        }

        public Task<int> SaveChangesAsync()
        {
            // In ADO.NET with stored procedures, each operation is immediately executed
            // So we don't need to track changes. This method is kept for interface consistency.
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}