using DemonstrationProject.Scripts.Services;
using DemonstrationProject.ViewModels;
using DemonstrationProject.Views.Controls;
using System.Windows;

namespace DemonstrationProject.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel context;
        public MainWindow()
        {
            InitializeComponent();
            context = new MainViewModel();
            DataContext = context;

        }
    }
}