using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DemonstrationProject.ViewModels
{
    class DataBasePageViewModel : INotifyPropertyChanged
    {
        public UserControl _curentPage;

        public UserControl CurrentPage
        {
            get { return _curentPage; }
            set { _curentPage = value; OnPropertyChanged(); }
        }

        public DataBasePageViewModel()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
