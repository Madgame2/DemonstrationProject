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
        private readonly UndoRedoService _undoRedoService = new UndoRedoService();

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

            // Инициализируем команды Undo/Redo
            UndoCommand = new RelayCommand(_ => _undoRedoService.Undo(), _ => _undoRedoService.CanUndo);
            RedoCommand = new RelayCommand(_ => _undoRedoService.Redo(), _ => _undoRedoService.CanRedo);

            // Создаем и настраиваем ShowcaseControl
            var showcaseControl = new ShowcaseControl();
            // Устанавливаем DataContext ShowcaseControl, передавая UnitOfWork и UndoRedoService
            showcaseControl.DataContext = new ShowcaseViewModel(App.UnitOfWork, _undoRedoService);

            // Создаем и настраиваем CartControl (если он тоже требует UndoRedoService, передайте его здесь)
            var cartControl = new CartControl();
            // Устанавливаем DataContext CartControl, передавая UnitOfWork и UserId
            cartControl.DataContext = new CartControlViewModel(App.UnitOfWork, App.UserId);

            _pageService.RegisterPage("ShowCase", showcaseControl);
            _pageService.RegisterPage("Cart", cartControl);

            NavigateToShowCaseCommand = new RelayCommand(_ => CurrentPage = _pageService.NavigateToPage("ShowCase"));
            NavigateToCartCommand = new RelayCommand(_ => CurrentPage = _pageService.NavigateToPage("Cart"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
