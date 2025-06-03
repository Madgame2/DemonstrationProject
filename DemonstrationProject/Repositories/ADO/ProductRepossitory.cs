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
        private readonly SqlTransaction _transaction;


        public ProductRepossitory(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task AddAsync(Product product)
        {
            var query = @"INSERT INTO Products (Name, Description, ImageSource, Price)
                            VALUES (@Name, @Description, @ImageSource, @Price)";

            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@ImageSource", product.ImageSource);
            command.Parameters.AddWithValue("@Price", product.Price);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Products WHERE Id = @Id";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();
            var query = "SELECT * from Products";
            using var command = new SqlCommand(query, _connection, _transaction);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    ImageSource = reader["ImageSource"].ToString(),
                    Price = (decimal)reader["Price"]
                });
            }
            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Products WHERE Id = @Id";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Product
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    ImageSource = reader["ImageSource"].ToString(),
                    Price = (decimal)reader["Price"]
                };
            }
            return null;
        }

        public async Task UpdateAsync(Product product)
        {
            var query = @"UPDATE Products SET Name = @Name, Description = @Description,
                      ImageSource = @ImageSource, Price = @Price WHERE Id = @Id";

            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@ImageSource", product.ImageSource);
            command.Parameters.AddWithValue("@Price", product.Price);
            await command.ExecuteNonQueryAsync();
        }
    }
}
