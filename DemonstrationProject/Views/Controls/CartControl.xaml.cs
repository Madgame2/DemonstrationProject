using System.Windows;
using System.Windows.Controls;
using DemonstrationProject.ViewModels;

namespace DemonstrationProject.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для CartControl.xaml
    /// </summary>
    public partial class CartControl : UserControl
    {
        public CartControl()
        {
            InitializeComponent();
            DataContext = new CartControlViewModel();
        }
    }
}
