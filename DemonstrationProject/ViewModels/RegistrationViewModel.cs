using DemonstrationProject.Commands;
using DemonstrationProject.Repositories.Interfaces;
using DemonstrationProject.Scripts.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DemonstrationProject.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;

        private readonly IPageService _pageService;
        private readonly IUserRepository _userRepository;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public RegistrationViewModel(IPageService pageService, IUserRepository userRepository)
        {
            _pageService = pageService;
            _userRepository = userRepository;
            RegisterCommand = new RelayCommand(_ => Register());
            NavigateToLoginCommand = new RelayCommand(_ => NavigateToLogin());
        }

        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (await _userRepository.UserExistsAsync(Username))
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (await _userRepository.RegisterAsync(Username, Password))
            {
                MessageBox.Show("Регистрация выполнена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigateToLogin();
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToLogin()
        {
            try
            {
                _pageService.NavigateToPage("Login");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переходе на страницу входа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
