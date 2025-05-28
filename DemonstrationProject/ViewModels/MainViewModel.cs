using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DemonstrationProject.Commands;

namespace DemonstrationProject.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _userName = string.Empty;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            LogoutCommand = new RelayCommand(_ => Logout());
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