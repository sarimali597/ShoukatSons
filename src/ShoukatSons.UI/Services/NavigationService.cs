// ─────────────────────────────────────────────────────
// File: Services/NavigationService.cs
// ─────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using ShoukatSons.UI.Services.Interfaces;
using WpfApplication = System.Windows.Application;
using WpfWindow = System.Windows.Window;
using WpfWindowStartupLocation = System.Windows.WindowStartupLocation;

namespace ShoukatSons.UI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _services;
        private readonly Stack<object> _stack = new();

        public NavigationService(IServiceProvider services)
        {
            _services = services;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object? Current => _stack.Count > 0 ? _stack.Peek() : null;

        public void Navigate(object viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            _stack.Push(viewModel);
            OnPropertyChanged(nameof(Current));
        }

        public object? GoBack()
        {
            if (_stack.Count > 1)
            {
                _ = _stack.Pop();
                OnPropertyChanged(nameof(Current));
            }
            return Current;
        }

        public void Show<TWindow>() where TWindow : WpfWindow
        {
            var w = _services.GetRequiredService<TWindow>();
            w.Owner = WpfApplication.Current.MainWindow;
            w.WindowStartupLocation = WpfWindowStartupLocation.CenterOwner;
            w.ShowInTaskbar = false;
            w.Show();
        }

        public bool? ShowDialog<TWindow>() where TWindow : WpfWindow
        {
            var w = _services.GetRequiredService<TWindow>();
            w.Owner = WpfApplication.Current.MainWindow;
            w.WindowStartupLocation = WpfWindowStartupLocation.CenterOwner;
            w.ShowInTaskbar = false;
            return w.ShowDialog();
        }

        public void CloseMainWindow() =>
            WpfApplication.Current?.MainWindow?.Close();

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
    }
}