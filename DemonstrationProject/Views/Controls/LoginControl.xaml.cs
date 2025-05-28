using System.Windows;
using System.Windows.Controls;

namespace DemonstrationProject.Views.Controls
{
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LogInViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }
    }
} 