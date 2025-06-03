using DemonstrationProject.Commands;
using DemonstrationProject.Models;
using DemonstrationProject.DB;
using DemonstrationProject.Scripts.Services;
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
        private readonly UndoRedoService _undoRedoService;
        public ObservableCollection<Product> Products { get; }

        // Команды для работы с витриной товаров
        public static RoutedUICommand SortProductsCommand = new RoutedUICommand(
            "Сортировать товары",
            "SortProducts",
            typeof(ShowcaseViewModel),
            new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control) }
        );

        public static RoutedUICommand FilterProductsCommand = new RoutedUICommand(
            "Фильтровать товары",
            "FilterProducts",
            typeof(ShowcaseViewModel),
            new InputGestureCollection { new KeyGesture(Key.F, ModifierKeys.Control) }
        );

        public static RoutedUICommand SearchProductsCommand = new RoutedUICommand(
            "Поиск товаров",
            "SearchProducts",
            typeof(ShowcaseViewModel),
            new InputGestureCollection { new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift) }
        );

        public static RoutedUICommand RefreshProductsCommand = new RoutedUICommand(
            "Обновить список товаров",
            "RefreshProducts",
            typeof(ShowcaseViewModel),
            new InputGestureCollection { new KeyGesture(Key.R, ModifierKeys.Control) }
        );

        public ICommand AddToCartCommand { get; }

        public ShowcaseViewModel(UnitOfWork unitOfWork, UndoRedoService undoRedoService)
        {
            _unitOfWork = unitOfWork;
            _undoRedoService = undoRedoService;
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

        private void OnAddToCart(Product product)
        {
            if (product == null) return;

            // Проверяем, вошел ли пользователь в систему
            if (App.UserId <= 0)
            {
                MessageBox.Show("Пожалуйста, войдите в систему, чтобы добавить товар в корзину.", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем отменяемую команду
            var addCommand = new AddCartItemCommand(_unitOfWork, new Cart
            {
                UserId = App.UserId,
                ProductId = product.Id,
                Product = product
            });

            // Выполняем команду через сервис UndoRedo
            _undoRedoService.ExecuteCommand(addCommand);

            // Обновляем UI корзины после выполнения команды
            // Важно: здесь мы не вызываем LoadCartItemsAsync напрямую в этом ViewModel,
            // потому что CartControlViewModel уже подписан на событие DataChanged UnitOfWork,
            // которое вызывается после CommitAsync в AddCartItemCommand.

            // MessageBox.Show($"Товар '{product.Name}' добавлен в корзину", "Успех",
            //     MessageBoxButton.OK, MessageBoxImage.Information);

            // Обновляем состояние команд Undo/Redo (теперь это делается в UserPageViewModel)
             // CommandManager.InvalidateRequerySuggested(); // Больше не нужно здесь
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
