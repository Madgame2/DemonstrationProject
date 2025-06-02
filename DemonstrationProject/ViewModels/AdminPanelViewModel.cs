using DemonstrationProject.Commands;
using DemonstrationProject.Scripts.Interfaces;
using DemonstrationProject.Scripts.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using DemonstrationProject.Views.Controls;

namespace DemonstrationProject.ViewModels
{
    class AdminPanelViewModel : INotifyPropertyChanged
    {
        private IPageService _pageService;

        public UserControl _curentPage;

        public UserControl CurentPage
        {
            get { return _curentPage; }
            set { _curentPage = value; OnPropertyChanged(); }
        }


        public ICommand NavigateToComandsPageCommand { get; }
        public ICommand NavigateToDBCommand { get; }

        public AdminPanelViewModel()
        {
            _pageService = new PageService();

            _pageService.RegisterPage("Commands", new ComandsControl());
            _pageService.RegisterPage("DB", new DataBaseControl());

            NavigateToComandsPageCommand = new RelayCommand(_ => CurentPage = _pageService.NavigateToPage("Commands"));
            NavigateToDBCommand = new RelayCommand(_ => CurentPage = _pageService.NavigateToPage("DB"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
