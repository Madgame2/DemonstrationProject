using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // Внешний ключ к Users
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Внешний ключ к Products
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
