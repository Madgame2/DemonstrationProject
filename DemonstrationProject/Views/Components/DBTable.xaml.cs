using System;
using System.Collections;
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
    /// Логика взаимодействия для DBTable.xaml
    /// </summary>
    public partial class DBTable : UserControl
    {
        private const double RowHeight = 30.0;
        private const double HeaderHeight = 30.0;

        public DBTable()
        {
            InitializeComponent();

        }

        public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(DBTable),
        new PropertyMetadata(null));

        public static readonly DependencyProperty MaxVisibleRowsProperty =
    DependencyProperty.Register(
        nameof(MaxVisibleRows),
        typeof(int),
        typeof(DBTable),
        new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsMeasure, OnMaxVisibleRowsChanged, CoerceMaxVisibleRows),
        ValidateMaxVisibleRows);

        private static bool ValidateMaxVisibleRows(object value)
        {
            return (int)value > 0;
        }

        private static object CoerceMaxVisibleRows(DependencyObject d, object baseValue)
        {
            int val = (int)baseValue;
            return val > 100 ? 100 : val;
        }

        private static void OnMaxVisibleRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DBTable)d;
            control.UpdateGridHeight();
        }

        private void UpdateGridHeight()
        {
            MainDataGrid.MaxHeight = HeaderHeight + MaxVisibleRows * RowHeight;
        }

        public int MaxVisibleRows
        {
            get => (int)GetValue(MaxVisibleRowsProperty);
            set => SetValue(MaxVisibleRowsProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
