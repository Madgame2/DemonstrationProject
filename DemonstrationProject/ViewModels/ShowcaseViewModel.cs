using DemonstrationProject.Commands;
using DemonstrationProject.Models;
using DemonstrationProject.DB;
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
        private readonly UnitOfWork _unitOfWork;
        public ObservableCollection<Product> Products { get; }

        public ICommand AddToCartCommand { get; }

        public ShowcaseViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Products = new ObservableCollection<Product>();

            AddToCartCommand = new RelayCommand<Product>(OnAddToCart);

            // Подписываемся на событие изменения данных
            _unitOfWork.DataChanged += OnDataChanged;

            // Загружаем данные при инициализации
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

        private async void OnAddToCart(Product product)
        {
            if (product == null) return;

            // Проверяем, вошел ли пользователь в систему
            if (App.UserId <= 0)
            {
                MessageBox.Show("Пожалуйста, войдите в систему, чтобы добавить товар в корзину.", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Создаем новую запись в корзине
                var cartItem = new Cart
                {
                    UserId = App.UserId, // Используем ID текущего пользователя из App
                    ProductId = product.Id,
                    Product = product,
                };

                // Добавляем в базу данных
                await _unitOfWork.Carts.AddAsync(cartItem);
                await _unitOfWork.CommitAsync();

                MessageBox.Show($"Товар '{product.Name}' добавлен в корзину", "Успех",
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
                MessageBox.Show($"Ошибка при добавлении товара в корзину: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
