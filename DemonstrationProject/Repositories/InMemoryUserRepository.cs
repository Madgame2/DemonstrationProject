using DemonstrationProject.Repositories.Interfaces;
using DemonstrationProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemonstrationProject.Scripts;

namespace DemonstrationProject.Repositories
{
    class InMemoryUserRepository : IUserRepository
    {
        private Dictionary<string, User> _users = new();
        public Task<int> AuthenticateAsync(string username, string password)
        {
            _users.TryGetValue(username, out var user);
            if (user == null) throw new UserNotFoundExaption();

            if(user.PasswordHash!=password) throw new UserNotFoundExaption();

            return Task.FromResult(user.Id);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterAsync(string username, string password)
        {
            if (_users.ContainsKey(username)) return Task.FromResult(false);

            _users[username] = new User { Id=GetFreeId(), PasswordHash = password, UserName = username };

            return Task.FromResult(true);
        }

        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(string username)=> Task.FromResult(_users.ContainsKey(username));

        private int GetFreeId()
        {
            int Id = 0;

            foreach (var user in _users.Values)
            {
                if (user.Id == Id) Id++;
            }

            return Id;
        }
    }
}
