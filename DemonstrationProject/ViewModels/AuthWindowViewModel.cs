using DemonstrationProject.Repositories;
using DemonstrationProject.Repositories.Interfaces;
using DemonstrationProject.Scripts.Interfaces;
using DemonstrationProject.Scripts.Services;
using DemonstrationProject.Views.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace DemonstrationProject.ViewModels
{
    public class AuthWindowViewModel : INotifyPropertyChanged
    {
        private UserControl? _currentPage;
        private readonly IPageService _pageService;
        private readonly IUserRepository _userRepository;

        public UserControl? CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public AuthWindowViewModel()
        {
            try
            {
                _pageService = new PageService();
                _userRepository = new InMemoryUserRepository();
                _pageService.PageChanged += OnPageChanged;


                var loginControl = new LoginControl { DataContext = new LogInViewModel(_pageService, _userRepository) };
                var registerControl = new RegisterControl { DataContext = new RegistrationViewModel(_pageService, _userRepository) };


                _pageService.RegisterPage("Login", loginControl);
                _pageService.RegisterPage("Register", registerControl);


                CurrentPage = loginControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPageChanged(object? sender, string pageName)
        {
            try
            {
                CurrentPage = _pageService.NavigateToPage(pageName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене страницы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToPage(string pageName)
        {
            try
            {
                CurrentPage = _pageService.NavigateToPage(pageName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при навигации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
