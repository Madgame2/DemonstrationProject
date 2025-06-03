using DemonstrationProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DemonstrationProject.Repositories.ADO
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public UserRepository(SqlConnection connection, SqlTransaction transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> AuthenticateAsync(string username, string password)
        {
            string sql = "SELECT Id, PasswordHash FROM Users WHERE UserName = @username";
            using var cmd = new SqlCommand(sql, _connection,_transaction);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return -1;

            int userId = (int)reader["Id"];
            return userId;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            if (await UserExistsAsync(username))
                return false;

            var sql = "INSERT INTO Users (UserName, PasswordHash) VALUES (@username, @passwordHash)";
            using var cmd = new SqlCommand(sql, _connection, _transaction);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@passwordHash", password);

            int affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE UserName = @username";
            using var cmd = new SqlCommand(sql, _connection, _transaction);
            cmd.Parameters.AddWithValue("@username", username);

            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
}
