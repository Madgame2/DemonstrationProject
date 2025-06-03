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
        public ICommand ConfirmPurchaseCommand { get; }

        public CartControlViewModel(UnitOfWork unitOfWork, int currentUserId)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserId;

            // Инициализируем команды
            AddToCartCommand = new RelayCommand<Product>(AddToCart);
            ConfirmPurchaseCommand = new RelayCommand(async _ => await ConfirmPurchaseAsync(), _ => CartItems.Any());

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

        private async Task ConfirmPurchaseAsync()
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Корзина пуста.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите подтвердить покупку и очистить корзину?", 
                                       "Подтверждение покупки", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                // Удаляем каждый элемент корзины
                foreach (var item in CartItems.ToList()) // Используем ToList() для избежания изменения коллекции во время итерации
                {
                    await _unitOfWork.Carts.RemoveAsync(item.Id);
                }

                // Сохраняем изменения
                await _unitOfWork.CommitAsync();

                // Очищаем UI коллекцию и обнуляем стоимость
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CartItems.Clear();
                    TotalCoast = 0;
                });

                MessageBox.Show("Покупка успешно подтверждена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show($"Ошибка при подтверждении покупки: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
