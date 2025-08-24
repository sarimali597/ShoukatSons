using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;    // unlocks GetRequiredService<T>()
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.Services.Navigation
{
    public class NavigationService : INavigationService, INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private ViewModelBase? _current;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? CurrentViewModelChanged;

        public ViewModelBase? CurrentViewModel
        {
            get => _current;
            private set
            {
                _current = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
                CurrentViewModelChanged?.Invoke();
            }
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
        }
    }
}