using DemonstrationProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.Repositories.Interfaces
{
    public interface ICartRerository
    {
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task RemoveAsync(int id);
        Task<Cart> GetByIdAsync(int id);
        Task<IEnumerable<Cart>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Cart>> GetAllAsync();
    }
}
