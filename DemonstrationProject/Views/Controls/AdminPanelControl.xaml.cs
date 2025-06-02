using System.Windows.Controls;
using DemonstrationProject.ViewModels;

namespace DemonstrationProject.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для AdminPanelControl.xaml
    /// </summary>
    public partial class AdminPanelControl : UserControl
    {
        public AdminPanelControl()
        {
            InitializeComponent();
            DataContext = new AdminPanelViewModel();
        }
    }
}
