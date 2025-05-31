using System.Windows.Controls;
using DemonstrationProject.ViewModels;

namespace DemonstrationProject.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserPageControl.xaml
    /// </summary>
    public partial class UserPageControl : UserControl
    {
        public UserPageControl()
        {
            InitializeComponent();
            DataContext = new UserPageViewModel();
        }
    }
}
