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
using DemonstrationProject.Commands;

namespace DemonstrationProject.ViewModels
{
    public class CartControlViewModel : INotifyPropertyChanged
    {
        private readonly UnitOfWork _unitOfWork;
        private decimal _totalCoast;
        private int _currentUserId; // ID текущего пользователя
        
        public decimal TotalCoast
        {
            get => _totalCoast;
            set { _totalCoast = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Cart> CartItems { get; } = new();

        public ICommand AddToCartCommand { get; }

        public CartControlViewModel(UnitOfWork unitOfWork, int currentUserId)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserId;

            // Инициализируем команду
            AddToCartCommand = new RelayCommand<Product>(AddToCart);

            // Подписываемся на событие изменения данных
            _unitOfWork.DataChanged += OnDataChanged;

            // Загружаем данные при инициализации
            _ = LoadCartItemsAsync();
        }

        private async void AddToCart(Product product)
        {
            if (product == null) return;

            try
            {
                // Создаем новую запись в корзине
                var cartItem = new Cart
                {
                    UserId = _currentUserId,
                    ProductId = product.Id,
                    Product = product
                };

                // Добавляем в базу данных
                await _unitOfWork.Carts.AddAsync(cartItem);
                await _unitOfWork.CommitAsync();

                // Обновляем UI
                await LoadCartItemsAsync();
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

        private async void OnDataChanged(object sender, EventArgs e)
        {
            await LoadCartItemsAsync();
        }

        private async Task LoadCartItemsAsync()
        {
            try
            {
                var cartItems = await _unitOfWork.Carts.GetByUserIdAsync(_currentUserId);
                
                // Обновляем UI в потоке UI
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CartItems.Clear();
                    decimal total = 0;

                    foreach (var cartItem in cartItems)
                    {
                        CartItems.Add(cartItem);
                        if (cartItem.Product != null)
                        {
                            total += cartItem.Product.Price;
                        }
                    }

                    TotalCoast = total;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке корзины: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
