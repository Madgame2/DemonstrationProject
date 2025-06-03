using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemonstrationProject.Views.Components
{
    /// <summary>
    /// Логика взаимодействия для FinaledField.xaml
    /// </summary>
    public partial class FinaledField : UserControl
    {
        public static readonly DependencyProperty FinalFealdProperty =
            DependencyProperty.Register(
                nameof(FinalFeald),
                typeof(string),
                typeof(FinaledField),
                new FrameworkPropertyMetadata("Default", null, CoerceFinalFeald),
                ValidateFinalFeald);

        public static readonly DependencyProperty FinalValueProperty =
            DependencyProperty.Register(
                nameof(FinalValue),
                typeof(decimal),
                typeof(FinaledField),
                new FrameworkPropertyMetadata(0m, null, CoerceFinalValue),
                ValidateFinalValue);

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(FinaledField),
                new FrameworkPropertyMetadata("", null, CoerceDescription),
                ValidateDescription);

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                nameof(ButtonText),
                typeof(string),
                typeof(FinaledField),
                new FrameworkPropertyMetadata("OK", null, CoerceButtonText),
                ValidateButtonText);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(FinaledField),
                new PropertyMetadata(null));

        public static readonly RoutedEvent SubmitEvent =
    EventManager.RegisterRoutedEvent(
        nameof(Submit),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(FinaledField));



        private static bool ValidateFinalFeald(object value) =>
    value is string str && !string.IsNullOrWhiteSpace(str);

        private static bool ValidateFinalValue(object value) =>
            value is decimal d && d >= 0; // например, только положительные значения

        private static bool ValidateDescription(object value) =>
            value is string str && str.Length <= 200; // ограничим длину

        private static bool ValidateButtonText(object value) =>
            value is string str && !string.IsNullOrWhiteSpace(str);


        private static object CoerceFinalFeald(DependencyObject d, object baseValue)
        {
            if (baseValue is string str)
                return str.Trim(); 
            return "Undefined";
        }

        private static object CoerceFinalValue(DependencyObject d, object baseValue)
        {
            if (baseValue is decimal dVal)
                return Math.Max(0, dVal); 
            return 0m;
        }

        private static object CoerceDescription(DependencyObject d, object baseValue)
        {
            if (baseValue is string str)
                return str.Length > 200 ? str.Substring(0, 200) : str;
            return string.Empty;
        }

        private static object CoerceButtonText(DependencyObject d, object baseValue)
        {
            if (baseValue is string str)
                return string.IsNullOrWhiteSpace(str) ? "Ок" : str.Trim();
            return "Ок";
        }

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public event RoutedEventHandler Submit
        {
            add =>AddHandler(SubmitEvent, value);
            remove =>RemoveHandler(SubmitEvent, value);
        }
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public decimal FinalValue
        {
            get => (decimal)GetValue(FinalValueProperty);
            set => SetValue(FinalValueProperty, value);
        }
        public string FinalFeald
        {
            get => (string)GetValue(FinalFealdProperty);
            set => SetValue(FinalFealdProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public FinaledField()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Command?.Execute(null);
        }
    }
}
