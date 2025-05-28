using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.Scripts
{
    public class UserNotFoundExaption : Exception
    {
        public UserNotFoundExaption(string? message) : base(message) { }
        public UserNotFoundExaption() : base() { }
    }
}
