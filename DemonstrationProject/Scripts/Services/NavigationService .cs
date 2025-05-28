using DemonstrationProject.Scripts.Interfaces;
using DemonstrationProject.ViewModels;
using DemonstrationProject.Views.Windows;
using System;
using System.Linq;
using System.Windows;

namespace DemonstrationProject.Scripts.Services
{
    public class NavigationService : INavigationService
    {
        public void CloseCurrentWindow()
        {
            var activeWindow = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(x => x.IsActive);
            
            if (activeWindow != null)
            {
                activeWindow.Close();
            }
        }

        public void OpenLoginWindow()
        {
            //var window = new LogInWindow
            //{
            //    DataContext = new LogInViewModel(this)
            //};
            //window.Show();
            //CloseCurrentWindow();
        }

        public void OpenRegisterWindow()
        {
            //var window = new RegistrationWindow
            //{
            //    DataContext = new RegistrationViewModel(this)
            //};
            //window.Show();
            //CloseCurrentWindow();
        }

        public void OpenMainWindow()
        {
            var window = new MainWindow();
            window.Show();
            CloseCurrentWindow();
        }
    }
}
