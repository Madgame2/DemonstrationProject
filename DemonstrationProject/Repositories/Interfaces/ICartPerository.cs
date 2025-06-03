using DemonstrationProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.Repositories.Interfaces
{
    public interface ICartPerository
    {
        void Add(Cart cart);
        void Remove(int id);
        Cart GetById(int id);
        IEnumerable<Cart> GetByUserId(int userId);
        IEnumerable<Cart> GetAll();
    }
}
