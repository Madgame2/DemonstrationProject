using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName {  get; set; }=String.Empty;
        public string PasswordHash { get; set; }=String.Empty;
    }
}
