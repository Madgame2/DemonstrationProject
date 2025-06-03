using DemonstrationProject.Commands;
using DemonstrationProject.DB;
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
using System.Windows;

namespace DemonstrationProject.ViewModels
{
    public class CommandsViewModel : INotifyPropertyChanged
    {
        private string _imagePath = string.Empty;
        private decimal _price;
        private string _description = string.Empty;
        private string _name = string.Empty;
        private readonly UnitOfWork _unitOfWork;
        private Product? _selectedProduct;

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
                    OnPropertyChanged();
                }
            }
        }
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    OnPropertyChanged();

                    // Копируем данные выбранного продукта в поля ввода
                    if (_selectedProduct != null)
                    {
                        Name = _selectedProduct.Name;
                        Description = _selectedProduct.Description;
                        Price = _selectedProduct.Price;
                        ImagePath = _selectedProduct.ImageSource;
                    }
                    else
                    {
                        // Очищаем поля, если ничего не выбрано
                        Name = string.Empty;
                        Description = string.Empty;
                        Price = 0;
                        ImagePath = string.Empty;
                    }
                }
            }
        }

        public ICommand LoadImageCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand UpdateProductCommand { get; }

        public ObservableCollection<Product> Products { get; } = new();

        public CommandsViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadImageCommand = new RelayCommand(_ => LoadImage());
            AddProductCommand = new RelayCommand(async _ => await AddNewProductAsync());
            DeleteProductCommand = new RelayCommand(async _ => await DeleteProductAsync(), _ => SelectedProduct != null);
            UpdateProductCommand = new RelayCommand(async _ => await UpdateProductAsync(), _ => SelectedProduct != null);

            // Подписываемся на событие изменения данных
            _unitOfWork.DataChanged += OnDataChanged;

            // Загружаем товары при инициализации
            _ = LoadProductsAsync();
        }

        private async void OnDataChanged(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                
                // Обновляем UI в потоке UI
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private async Task AddNewProductAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || 
                string.IsNullOrWhiteSpace(ImagePath) || Price <= 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newProduct = new Product
                {
                    Name = Name,
                    Description = Description,
                    ImageSource = ImagePath,
                    Price = Price
                };

                await _unitOfWork.Products.AddAsync(newProduct);
                await _unitOfWork.CommitAsync();

                // Очищаем поля
                Name = string.Empty;
                Description = string.Empty;
                ImagePath = string.Empty;
                Price = 0;
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show($"Ошибка при откате изменений: {rollbackEx.Message}", 
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить товар '{SelectedProduct.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _unitOfWork.Products.DeleteAsync(SelectedProduct.Id);
                await _unitOfWork.CommitAsync();
                SelectedProduct = null;
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show($"Ошибка при откате изменений: {rollbackEx.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show($"Ошибка при удалении товара: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateProductAsync()
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show("Выберите товар для изменения", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) ||
                string.IsNullOrWhiteSpace(ImagePath) || Price <= 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SelectedProduct.Name = Name;
                SelectedProduct.Description = Description;
                SelectedProduct.ImageSource = ImagePath;
                SelectedProduct.Price = Price;

                await _unitOfWork.Products.UpdateAsync(SelectedProduct);
                await _unitOfWork.CommitAsync();

                MessageBox.Show("Товар успешно обновлен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show($"Ошибка при откате изменений: {rollbackEx.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show($"Ошибка при обновлении товара: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
