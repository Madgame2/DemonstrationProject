using System.Configuration;
using System.Data;
using System.Windows;
using DemonstrationProject.DB;
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

        public static UnitOfWork UnitOfWork { get; private set; }
        public static int UserId { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при инициализации базы данных: " + ex.Message);
                Shutdown(); 
            }

            string connectionString = ConfigurationManager.ConnectionStrings["DemonstrationDB"].ConnectionString;
            UnitOfWork = new UnitOfWork(connectionString);

            var pageService = new PageService();

            var maInWindow = new MainWindow();
            var Window = new AuthWindow();
            Window.Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            UnitOfWork?.Dispose();
            base.OnExit(e);
        }
    }
}
