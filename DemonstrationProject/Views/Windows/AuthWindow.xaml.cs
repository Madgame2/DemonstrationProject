using DemonstrationProject.ViewModels;
using System.Windows;

namespace DemonstrationProject.Views.Windows
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            DataContext = new AuthWindowViewModel();
        }
    }
} 