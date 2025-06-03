using DemonstrationProject.Models;
using DemonstrationProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DemonstrationProject.Repositories.ADO
{
    public class ProductRepossitory : IProductRepository
    {
        private readonly SqlConnection _connection;
        private readonly Func<SqlTransaction> _getTransaction;

        public ProductRepossitory(SqlConnection connection, Func<SqlTransaction> getTransaction)
        {
            _connection = connection;
            _getTransaction = getTransaction;
        }

        public async Task AddAsync(Product entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var command = new SqlCommand("INSERT INTO Products (Name, Description, Price, ImageSource) VALUES (@Name, @Description, @Price, @ImageSource)", _connection))
            {
                // Получаем текущую транзакцию перед выполнением команды
                command.Transaction = _getTransaction();

                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@Description", entity.Description);
                command.Parameters.AddWithValue("@Price", entity.Price);
                command.Parameters.AddWithValue("@ImageSource", entity.ImageSource);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(Product entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var command = new SqlCommand("UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, ImageSource = @ImageSource WHERE Id = @Id", _connection))
            {
                // Получаем текущую транзакцию перед выполнением команды
                command.Transaction = _getTransaction();

                command.Parameters.AddWithValue("@Id", entity.Id);
                command.Parameters.AddWithValue("@Name", entity.Name);
                command.Parameters.AddWithValue("@Description", entity.Description);
                command.Parameters.AddWithValue("@Price", entity.Price);
                command.Parameters.AddWithValue("@ImageSource", entity.ImageSource);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", _connection))
            {
                // Получаем текущую транзакцию перед выполнением команды
                command.Transaction = _getTransaction();

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using (var command = new SqlCommand("SELECT Id, Name, Description, Price, ImageSource FROM Products WHERE Id = @Id", _connection))
            {
                // Получаем текущую транзакцию перед выполнением команды
                command.Transaction = _getTransaction();

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageSource = reader.GetString(4)
                        };
                    }
                    return null;
                }
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();
            using (var command = new SqlCommand("SELECT Id, Name, Description, Price, ImageSource FROM Products", _connection))
            {
                // Получаем текущую транзакцию перед выполнением команды
                command.Transaction = _getTransaction();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageSource = reader.GetString(4)
                        });
                    }
                }
            }
            return products;
        }
    }
}
