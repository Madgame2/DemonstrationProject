using DemonstrationProject.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DemonstrationProject.Scripts.Services
{
    public class PageService : IPageService
    {
        private readonly Dictionary<string, UserControl> _pages = new();
        private UserControl? _currentPage;

        public event EventHandler<string>? PageChanged;

        public UserControl NavigateToPage(string pageName)
        {
            if (_pages.TryGetValue(pageName, out var page))
            {
                if (_currentPage != page)
                {
                    _currentPage = page;
                    PageChanged?.Invoke(this, pageName);
                }
                return page;
            }
            
            throw new ArgumentException($"Страница с именем {pageName} не найдена");
        }

        public void RegisterPage(string pageName, UserControl page)
        {
            if (_pages.ContainsKey(pageName))
            {
                throw new ArgumentException($"Страница с именем {pageName} уже зарегистрирована");
            }

            _pages[pageName] = page;
        }
    }
} 