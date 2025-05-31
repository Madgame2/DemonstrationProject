using DemonstrationProject.Commands;
using DemonstrationProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DemonstrationProject.ViewModels
{
    public class ShowcaseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Product> Products { get; }

        public ICommand AddToCartCommand { get; }

        public ShowcaseViewModel()
        {
            Products = new ObservableCollection<Product>();

            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });
            Products.Add(new Product { Name = "somename", Description = "fdsfdsdf", ImageSource = "", Price = 144 });

            AddToCartCommand = new RelayCommand<Product>(OnAddToCart);

        }

        private void OnAddToCart(Product product)
        {
            // Здесь логика добавления в корзину
            MessageBox.Show($"Добавлено в корзину: {product.Name}");
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
