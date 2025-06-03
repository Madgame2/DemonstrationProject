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
    public class CartRepository : ICartPerository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public CartRepository(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task AddAsync(Cart cart)
        {
            var query = @"INSERT INTO Carts (UserID, ProductID) VALUES (@UserID, @ProductID)";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@UserID", cart.UserId);
            command.Parameters.AddWithValue("@ProductID", cart.ProductId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            var carts = new List<Cart>();

            string sql = @"
            SELECT c.Id AS CartId, c.UserId, c.ProductId,
                   u.Id AS UserId, u.UserName, u.PasswordHash,
                   p.Id AS ProductId, p.Name, p.Description, p.ImageSource, p.Price
            FROM Carts c
            JOIN Users u ON c.UserId = u.Id
            JOIN Products p ON c.ProductId = p.Id";

            using var cmd = new SqlCommand(sql, _connection, _transaction);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                carts.Add(MapCart(reader));
            }

            return carts;
        }

        public async Task<Cart> GetByIdAsync(int id)
        {
            var query = @"
            SELECT c.Id AS CartId, c.UserId, c.ProductId,
                   u.Id AS UserId, u.UserName, u.PasswordHash,
                   p.Id AS ProductId, p.Name, p.Description, p.ImageSource, p.Price
            FROM Carts c
            JOIN Users u ON c.UserId = u.Id
            JOIN Products p ON c.ProductId = p.Id
            WHERE c.Id = @id";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return MapCart(reader);
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

            using var cmd = new SqlCommand(sql, _connection, _transaction);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                carts.Add(MapCart(reader));
            }

            return carts;
        }

        public async Task RemoveAsync(int id)
        {
            var query = "DELETE FROM Carts WHERE Id = @Id";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
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
    }
}
