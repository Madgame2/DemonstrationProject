using DemonstrationProject.Commands;
using DemonstrationProject.Scripts.Interfaces;
using DemonstrationProject.Scripts.Services;
using DemonstrationProject.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DemonstrationProject.ViewModels
{
    public class UserPageViewModel : INotifyPropertyChanged
    {
        private IPageService _pageService;

        private UserControl _currentPage;

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand NavigateToShowCaseCommand { get; }
        public ICommand NavigateToCartCommand { get; }

        public UserControl CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public UserPageViewModel()
        {
            _pageService = new PageService();

            _pageService.RegisterPage("ShowCase", new ShowcaseControl());

            NavigateToShowCaseCommand = new RelayCommand(_ => CurrentPage = _pageService.NavigateToPage("ShowCase"));
            NavigateToCartCommand = new RelayCommand(_ => CurrentPage = _pageService.NavigateToPage("Cart"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
