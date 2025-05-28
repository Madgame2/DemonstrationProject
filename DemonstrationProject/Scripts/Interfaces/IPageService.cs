using System;
using System.Windows.Controls;

namespace DemonstrationProject.Scripts.Interfaces
{
    public interface IPageService
    {
        UserControl NavigateToPage(string pageName);
        void RegisterPage(string pageName, UserControl page);
        event EventHandler<string>? PageChanged;
    }
} 