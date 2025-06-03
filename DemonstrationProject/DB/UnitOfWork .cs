using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemonstrationProject.Repositories.Interfaces;
using DemonstrationProject.Repositories.ADO;
using System.Windows;

namespace DemonstrationProject.DB
{
    public class UnitOfWork : IDisposable
    {
        public event EventHandler DataChanged;

        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;
        private bool _isDisposed;
        private readonly object _transactionLock = new object();

        public IUserRepository Users { get; }
        public ICartRerository Carts { get; }
        public IProductRepository Products { get; }

        // Метод для получения текущей транзакции
        internal SqlTransaction GetCurrentTransaction()
        {
            lock (_transactionLock)
            {
                if (_isDisposed) throw new ObjectDisposedException(nameof(UnitOfWork));
                if (_transaction == null) throw new InvalidOperationException("Transaction is null or has not been started.");
                return _transaction;
            }
        }

        public UnitOfWork(string connectionString)
        {
            // Добавляем поддержку MARS
            if (!connectionString.Contains("MultipleActiveResultSets"))
            {
                connectionString += ";MultipleActiveResultSets=True";
            }

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            
            // Создаем начальную транзакцию
            lock (_transactionLock)
            {
                 _transaction = _connection.BeginTransaction();
            }

            // Передаем функцию для получения текущей транзакции в репозитории
            Users = new UserRepository(_connection, GetCurrentTransaction);
            Carts = new CartRepository(_connection, GetCurrentTransaction);
            Products = new ProductRepossitory(_connection, GetCurrentTransaction);
        }

        public async Task CommitAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(UnitOfWork));
            
            SqlTransaction? currentTransaction = null;
            lock (_transactionLock)
            {
                currentTransaction = _transaction;
                if (currentTransaction == null) throw new InvalidOperationException("Transaction is null");
            }

            try
            {
                // Используем синхронный Commit внутри lock
                lock (_transactionLock)
                {
                    currentTransaction.Commit();
                    // Сразу создаем новую транзакцию после коммита в том же lock
                    _transaction = _connection.BeginTransaction();
                }
                
                MessageBox.Show("Commit выполнен, вызываю событие DataChanged");
                OnDataChanged();
            }catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в CommitAsync: {ex.Message}");
                // Rollback также должен быть синхронизирован
                await Task.Run(() => Rollback()); // Выполняем синхронный Rollback в пуле потоков
                throw;
            }
        }

        public async Task RollbackAsync()
        {
             await Task.Run(() => Rollback()); // Выполняем синхронный Rollback в пуле потоков
        }

        private void Rollback()
        {
            if (_isDisposed) return; 

            SqlTransaction? currentTransaction = null;
            lock (_transactionLock)
            {
                currentTransaction = _transaction;
                _transaction = null; // Сбрасываем транзакцию перед откатом
            }

            if (currentTransaction == null) return;

            try
            {
                 currentTransaction.Rollback();
            }
            catch (InvalidOperationException)
            {
                // Транзакция уже завершена, игнорируем
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Ошибка при Rollback: {ex.Message}");
            }
            finally
            {
                // Гарантируем создание новой транзакции после отката
                 lock (_transactionLock)
                 {
                     currentTransaction.Dispose(); // Освобождаем ресурсы старой транзакции
                     _transaction = _connection.BeginTransaction();
                 }
            }
        }

        protected virtual void OnDataChanged()
        {
            MessageBox.Show("OnDataChanged вызван");
            DataChanged?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Событие DataChanged вызвано");
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            // Синхронизируем Dispose
            lock(_transactionLock)
            {
                try
                {
                    _transaction?.Dispose();
                    _connection?.Dispose();
                }
                 catch (Exception ex)
                 {
                     MessageBox.Show($"Ошибка при Dispose: {ex.Message}");
                 }
                 finally
                 {
                    _isDisposed = true;
                    _transaction = null;
                 }
            }
        }
    }
}
