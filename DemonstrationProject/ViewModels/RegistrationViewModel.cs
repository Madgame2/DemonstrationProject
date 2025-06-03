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
            RegisterCommand = new RelayCommand(async _ => await Register());
            NavigateToLoginCommand = new RelayCommand(_ => NavigateToLogin());
        }

        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var uow = App.UnitOfWork;
            try
            {
                bool exists = await uow.Users.UserExistsAsync(Username);

                if (exists)
                {
                    MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = await uow.Users.RegisterAsync(Username, Password);
                if (result)
                {
                    await uow.CommitAsync();
                    MessageBox.Show("Регистрация выполнена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigateToLogin();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await uow.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show($"Ошибка при откате изменений: {rollbackEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
