using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using DemonstrationProject.ViewModels;

namespace DemonstrationProject.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ComandsControl.xaml
    /// </summary>
    public partial class ComandsControl : UserControl
    {
        public ComandsControl()
        {
            InitializeComponent();
            DataContext = new CommandsViewModel(App.UnitOfWork);
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string newText = GetTextAfterInput(textBox, e.Text);
            e.Handled = !IsValidDecimalInput(newText);
        }

        private void PriceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Блокируем пробел
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private string GetTextAfterInput(TextBox textBox, string input)
        {
            int start = textBox.SelectionStart;
            int length = textBox.SelectionLength;

            string originalText = textBox.Text;
            string newText = originalText.Remove(start, length).Insert(start, input);

            return newText;
        }

        private bool IsValidDecimalInput(string input)
        {
            // Заменяем точку на запятую, если пользователь использует точку
            input = input.Replace('.', ',');

            // Разрешаем до 18 цифр до запятой и до 2 после
            return Regex.IsMatch(input, @"^\d{0,18}(,\d{0,2})?$");
        }
    }
}
