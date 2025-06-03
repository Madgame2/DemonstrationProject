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
    public class CartRepository : ICartRerository
    {
        private readonly SqlConnection _connection;
        private readonly Func<SqlTransaction> _getTransaction;

        public CartRepository(SqlConnection connection, Func<SqlTransaction> getTransaction)
        {
            _connection = connection;
            _getTransaction = getTransaction;
        }

        public async Task AddAsync(Cart entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var command = new SqlCommand("INSERT INTO Carts (UserId, ProductId) VALUES (@UserId, @ProductId)", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@UserId", entity.UserId);
                command.Parameters.AddWithValue("@ProductId", entity.ProductId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(Cart entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var command = new SqlCommand("UPDATE Carts SET UserId = @UserId, ProductId = @ProductId WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", entity.Id);
                command.Parameters.AddWithValue("@UserId", entity.UserId);
                command.Parameters.AddWithValue("@ProductId", entity.ProductId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var command = new SqlCommand("DELETE FROM Carts WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Cart?> GetByIdAsync(int id)
        {
            using (var command = new SqlCommand("SELECT Id, UserId, ProductId FROM Carts WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Cart
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                        };
                    }
                    return null;
                }
            }
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            var carts = new List<Cart>();
            using (var command = new SqlCommand("SELECT Id, UserId, ProductId FROM Carts", _connection))
            {
                command.Transaction = _getTransaction();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        carts.Add(new Cart
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                        });
                    }
                }
            }
            return carts;
        }

        public async Task<IEnumerable<Cart>> GetByUserIdAsync(int userId)
        {
            var carts = new List<Cart>();

            var sql = @"
            SELECT c.Id AS CartId, c.UserId, c.ProductId,
                   u.Id AS UserId, u.UserName, u.PasswordHash,
                   p.Id AS ProductId, p.Name, p.Description, p.ImageSource, p.Price
            FROM Carts c
            JOIN Users u ON c.UserId = u.Id
            JOIN Products p ON c.ProductId = p.Id
            WHERE c.UserId = @userId";

            using var cmd = new SqlCommand(sql, _connection, _getTransaction());
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                carts.Add(MapCart(reader));
            }

            return carts;
        }

        private Cart MapCart(SqlDataReader reader)
        {
            return new Cart
            {
                Id = (int)reader["CartId"],
                UserId = (int)reader["UserId"],
                ProductId = (int)reader["ProductId"],
                User = new User
                {
                    Id = (int)reader["UserId"],
                    UserName = (string)reader["UserName"],
                    PasswordHash = (string)reader["PasswordHash"]
                },
                Product = new Product
                {
                    Id = (int)reader["ProductId"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    ImageSource = (string)reader["ImageSource"],
                    Price = (decimal)reader["Price"]
                }
            };
        }

        public  async Task RemoveAsync(int id)
        {
            using (var command = new SqlCommand("DELETE FROM Carts WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
