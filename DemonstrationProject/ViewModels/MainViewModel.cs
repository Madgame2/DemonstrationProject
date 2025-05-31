using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using DemonstrationProject.Commands;
using DemonstrationProject.Scripts.Services;
using DemonstrationProject.Views.Controls;

namespace DemonstrationProject.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private UserControl _currentPage;
        private PageService _pageService;

        public ICommand LogoutCommand { get; }

        public ICommand NavigateToShowcaseCommand { get; }
        public ICommand NavigateToAdminPanelCommand { get; }

        public UserControl CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _pageService = new PageService();

            _pageService.RegisterPage("Showcase", new UserPageControl());

            LogoutCommand = new RelayCommand(_ => Logout());
            NavigateToShowcaseCommand = new RelayCommand(_ => CurrentPage = _pageService.NavigateToPage("Showcase"));
            NavigateToAdminPanelCommand = new RelayCommand(_ => CurrentPage = _pageService.NavigateToPage("AdminPanel"));
        }

        private void Logout()
        {
            // Открываем окно авторизации
            var authWindow = new Views.Windows.AuthWindow();
            authWindow.Show();

            // Закрываем главное окно
            if (System.Windows.Application.Current.Windows.Count > 0)
            {
                System.Windows.Application.Current.Windows[0].Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
} 