using DemonstrationProject.Commands;
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

        public LogInViewModel(IPageService pageService)
        {
            _pageService = pageService;
            LogInCommand = new RelayCommand(_ => Login());
            NavigateToRegisterCommand = new RelayCommand(_ => NavigateToRegister());
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Вход выполнен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Открываем главное окно
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Закрываем окно авторизации
            if (Application.Current.Windows.Count > 0)
            {
                Application.Current.Windows[0].Close();
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
