using DemonstrationProject.Models;
using DemonstrationProject.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DemonstrationProject.Repositories.ADO
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection _connection;
        private readonly Func<SqlTransaction> _getTransaction;

        public UserRepository(SqlConnection connection, Func<SqlTransaction> getTransaction)
        {
            _connection = connection;
            _getTransaction = getTransaction;
        }

        public async Task<int> AuthenticateAsync(string username, string password)
        {
            // Вставьте сюда логику аутентификации пользователя
            // Например, поиск пользователя по логину и проверка пароля (с хешированием)

            using (var command = new SqlCommand("SELECT Id FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", password);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return (int)result;
                }
                throw new Exception("User not found or invalid password.");
            }
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            // Проверяем, существует ли пользователь с таким именем
            if (await UserExistsAsync(username))
            {
                return false; // Пользователь уже существует
            }

            // Вставляем нового пользователя
            using (var command = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", password);

                var affectedRows = await command.ExecuteNonQueryAsync();
                return affectedRows > 0;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            using (var command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Username", username);

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task AddAsync(User entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var command = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Username", entity.UserName);
                command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(User entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var command = new SqlCommand("UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", entity.Id);
                command.Parameters.AddWithValue("@Username", entity.UserName);
                command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using (var command = new SqlCommand("SELECT Id, Username, PasswordHash FROM Users WHERE Id = @Id", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            UserName = reader.GetString(1),
                            PasswordHash = reader.GetString(2)
                        };
                    }
                    return null;
                }
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using (var command = new SqlCommand("SELECT Id, Username, PasswordHash FROM Users WHERE Username = @Username", _connection))
            {
                command.Transaction = _getTransaction();
                command.Parameters.AddWithValue("@Username", username);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            UserName = reader.GetString(1),
                            PasswordHash = reader.GetString(2) // В реальном приложении здесь должно быть хеширование
                        };
                    }
                    return null;
                }
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();
            using (var command = new SqlCommand("SELECT Id, Username, PasswordHash FROM Users", _connection))
            {
                command.Transaction = _getTransaction();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            UserName = reader.GetString(1),
                            PasswordHash = reader.GetString(2)
                        });
                    }
                }
            }
            return users;
        }

        // Методы GetAllAsync, FindAsync и т.д. также должны использовать GetTransaction()
    }
}
