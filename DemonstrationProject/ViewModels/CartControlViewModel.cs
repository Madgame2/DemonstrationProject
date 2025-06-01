using DemonstrationProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DemonstrationProject.ViewModels
{
    public class CartControlViewModel : INotifyPropertyChanged
    {
        private decimal _totalCoast;
        
        public decimal TotalCoast
        {
            get => _totalCoast;
            set { _totalCoast = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Product> Products { get; } = new();

        public CartControlViewModel()
        {
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            TotalCoast = 288;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
