using System.Configuration;
using System.Data;
using System.Windows;
using DemonstrationProject.Scripts.Services;
using DemonstrationProject.ViewModels;
using DemonstrationProject.Views.Controls;
using DemonstrationProject.Views.Windows;

namespace DemonstrationProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var pageService = new PageService();

            var maInWindow = new MainWindow();
            var Window = new AuthWindow();
            maInWindow.Show();
        }
    }
}
