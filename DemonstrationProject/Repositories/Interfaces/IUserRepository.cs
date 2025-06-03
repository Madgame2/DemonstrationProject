using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemonstrationProject.Models;

namespace DemonstrationProject.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterAsync(string username, string password);
        Task<int> AuthenticateAsync(string username, string password);
        Task<bool> UserExistsAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task UpdateAsync(User user);
    }
}
