using DemonstrationProject.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DemonstrationProject.Models;

namespace DemonstrationProject.ViewModels
{
    public class CommandsViewModel : INotifyPropertyChanged
    {
        private string _imagePath = string.Empty;
        private decimal _price;
        private string _description = string.Empty;
        private string _name =string.Empty;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(); // если используешь INotifyPropertyChanged
                }
            }
        }
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }


        public ICommand LoadImageCommand { get; }


        public ObservableCollection<Product> Products { get; } = new();
        public CommandsViewModel()
        {
            LoadImageCommand = new RelayCommand(_=>LoadImage());

            Products.Add(new Product { Id = 1,Name="fdsfs",Description="fsdfdsf",ImageSource="ffdfsf" });
        }


        private void LoadImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
