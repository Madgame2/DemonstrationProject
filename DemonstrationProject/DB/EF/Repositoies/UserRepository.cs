using DemonstrationProject.Models;
using DemonstrationProject.Repositories.Interfaces;
using DemonstrationProject.Scripts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.DB.EF.Repositoies
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == password);

            return user?.Id ?? throw new UserNotFoundExaption(); 
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == username))
                return false;

            var newUser = new User
            {
                UserName = username,
                PasswordHash = password
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Проверка: существует ли пользователь с таким логином
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }
    }
}
