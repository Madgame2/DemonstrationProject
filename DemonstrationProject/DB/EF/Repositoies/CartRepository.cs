using DemonstrationProject.Models;
using DemonstrationProject.Repositories.ADO;
using DemonstrationProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.DB.EF.Repositoies
{
    public class CartRepository : ICartPerository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Cart cart)
        {
            _context.Carts.Add(cart);
        }

        public IEnumerable<Cart> GetAll()
        {
            return _context.Carts
                .Include(c => c.User)      // загружаем связанные данные пользователя
                .Include(c => c.Product)   // загружаем связанные продукты
                .ToList();
        }

        public Cart? GetById(int id)
        {
            return _context.Carts
                .Include(c => c.User)
                .Include(c => c.Product)
                .FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Cart> GetByUserId(int userId)
        {
            return _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public void Remove(int id)
        {
            var cart = _context.Carts.Find(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
        }
    }
}
