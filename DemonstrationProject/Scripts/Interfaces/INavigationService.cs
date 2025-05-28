using System;

namespace DemonstrationProject.Scripts.Interfaces
{
    public interface INavigationService
    {
        void OpenLoginWindow();
        void OpenRegisterWindow();
        void OpenMainWindow();
        void CloseCurrentWindow();
    }
}
