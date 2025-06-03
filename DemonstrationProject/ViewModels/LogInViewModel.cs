using DemonstrationProject.Commands;
using DemonstrationProject.DB;
using DemonstrationProject.Scripts;
using DemonstrationProject.Scripts.Interfaces;
using DemonstrationProject.Views.Windows;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DemonstrationProject.ViewModels
{
    public class LogInViewModel : INotifyPropertyChanged
    {
        private string _userName = string.Empty;
        private string _password = string.Empty;

        private readonly IPageService _pageService;
        private readonly UnitOfWork _unitOfWork;

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LogInCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LogInViewModel(IPageService pageService, UnitOfWork unitOfWork)
        {
            _pageService = pageService;
            _unitOfWork = unitOfWork;
            LogInCommand = new RelayCommand(async _ => await Login());
            NavigateToRegisterCommand = new RelayCommand(_ => NavigateToRegister());
        }

        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            try
            {
                var userId = await _unitOfWork.Users.AuthenticateAsync(UserName, Password);
                
                // Открываем главное окно
                var mainWindow = new MainWindow();
                mainWindow.Show();

                var currentWindow = App.Current.MainWindow;
                
                App.Current.MainWindow = mainWindow;
                currentWindow.Close();

            }
            catch (UserNotFoundExaption)
            {
                MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToRegister()
        {
            try
            {
                _pageService.NavigateToPage("Register");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переходе на страницу регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
