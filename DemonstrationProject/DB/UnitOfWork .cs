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
        private bool _isDisposed;

        public IUserRepository Users { get; }
        public ICartRerository Carts { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(string connectionString)
        {
            // Добавляем поддержку MARS
            if (!connectionString.Contains("MultipleActiveResultSets"))
            {
                connectionString += ";MultipleActiveResultSets=True";
            }

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Users = new UserRepository(_connection, _transaction);
            Carts = new CartRepository(_connection, _transaction);
            Products = new ProductRepossitory(_connection, _transaction);
        }

        public async Task CommitAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(UnitOfWork));
            if (_transaction == null) throw new InvalidOperationException("Transaction is null");

            try
            {
                await _transaction.CommitAsync();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception)
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(UnitOfWork));
            if (_transaction == null) return;

            try
            {
                await _transaction.RollbackAsync();
            }
            catch (InvalidOperationException)
            {
                // Транзакция уже завершена
            }
            finally
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            try
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }
}
