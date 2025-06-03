using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemonstrationProject.Repositories.Interfaces;
using DemonstrationProject.Repositories.ADO;

namespace DemonstrationProject.DB
{
    public class UnitOfWork : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public IUserRepository Users { get; }
        public ICartRerository Carts { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Users = new UserRepository(_connection, _transaction);
            Carts = new CartRepository(_connection, _transaction);
            Products = new ProductRepossitory(_connection, _transaction);

        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction = _connection.BeginTransaction();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction = _connection.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
