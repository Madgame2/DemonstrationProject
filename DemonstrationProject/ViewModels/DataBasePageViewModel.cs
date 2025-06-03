using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DemonstrationProject.Models;
using System.Windows.Input;
using DemonstrationProject.Commands;
using DemonstrationProject.DB;
using System.Windows;

namespace DemonstrationProject.ViewModels
{
    class DataBasePageViewModel : INotifyPropertyChanged
    {
        private readonly UnitOfWork _unitOfWork;
        private UserControl _curentPage;
        private ObservableCollection<Product> _products;
        private ObservableCollection<User> _Users;
        private ObservableCollection<Cart> _carts;
        private object _selectedCollection;

        public UserControl CurrentPage
        {
            get { return _curentPage; }
            set { _curentPage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> Users
        {
            get => _Users;
            set
            {
                _Users = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Cart> Carts
        {
            get => _carts;
            set
            {
                _carts = value;
                OnPropertyChanged();
            }
        }

        public object SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                _selectedCollection = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToUserCommand { get; }
        public ICommand ToCartCommand { get; }
        public ICommand ToProductCommand { get; }
        public ICommand SynchronizeCommand { get; }

        public DataBasePageViewModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Products = new ObservableCollection<Product>();
            Users = new ObservableCollection<User>();
            Carts = new ObservableCollection<Cart>();

            ToUserCommand = new RelayCommand(_=>{ SelectedCollection = Users; System.Diagnostics.Debug.WriteLine($"SelectedCollection set to Users. Count: {Users.Count}"); });
            ToCartCommand = new RelayCommand(_ => { SelectedCollection = Carts; System.Diagnostics.Debug.WriteLine($"SelectedCollection set to Carts. Count: {Carts.Count}"); });
            ToProductCommand = new RelayCommand(_ => { SelectedCollection = Products; System.Diagnostics.Debug.WriteLine($"SelectedCollection set to Products. Count: {Products.Count}"); });
            SynchronizeCommand = new RelayCommand(async _ => await SynchronizeDataAsync());

            // Подписываемся на событие изменения данных
            _unitOfWork.DataChanged += OnDataChanged;

            // Загружаем данные при инициализации
            _ = LoadDataAsync();
        }

        private async void OnDataChanged(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Загружаем все данные параллельно
                var productsTask = _unitOfWork.Products.GetAllAsync();
                var usersTask = _unitOfWork.Users.GetAllAsync();
                var cartsTask = _unitOfWork.Carts.GetAllAsync();

                await Task.WhenAll(productsTask, usersTask, cartsTask);

                // Обновляем UI в потоке UI
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Обновляем коллекции
                    Products.Clear();
                    foreach (var product in productsTask.Result)
                    {
                        Products.Add(product);
                    }
                    System.Diagnostics.Debug.WriteLine($"Loaded {Products.Count} products.");

                    Users.Clear();
                    foreach (var user in usersTask.Result)
                    {
                        Users.Add(user);
                    }
                    System.Diagnostics.Debug.WriteLine($"Loaded {Users.Count} users.");

                    Carts.Clear();
                    foreach (var cart in cartsTask.Result)
                    {
                        Carts.Add(cart);
                    }
                    System.Diagnostics.Debug.WriteLine($"Loaded {Carts.Count} carts.");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SynchronizeDataAsync()
        {
            try
            {
                // Сохраняем изменения для всех элементов в коллекциях
                foreach (var product in Products)
                {
                    await _unitOfWork.Products.UpdateAsync(product);
                }
                foreach (var user in Users)
                {
                    await _unitOfWork.Users.UpdateAsync(user);
                }
                foreach (var cart in Carts)
                {
                    await _unitOfWork.Carts.UpdateAsync(cart);
                }

                await _unitOfWork.CommitAsync();
                MessageBox.Show("Данные успешно синхронизированы", "Успех",
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
                System.Diagnostics.Debug.WriteLine($"Error synchronizing data: {ex.Message}");
                MessageBox.Show($"Ошибка при синхронизации данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
