using System.Windows.Controls;
using DemonstrationProject.ViewModels;

namespace DemonstrationProject.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для DataBaseControl.xaml
    /// </summary>
    public partial class DataBaseControl : UserControl
    {
        public DataBaseControl()
        {
            InitializeComponent();
            DataContext = new DataBasePageViewModel(App.UnitOfWork);
        }
    }
}
