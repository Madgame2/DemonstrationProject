using System.Windows;
using System.Windows.Controls;

namespace DemonstrationProject.Views.Controls
{
    public partial class RegisterControl : UserControl
    {
        public RegisterControl()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegistrationViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }
    }
} 