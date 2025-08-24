using ShoukatSons.POS.Wpf.ViewModels.Base;
using System;

namespace ShoukatSons.POS.Wpf.Services.Navigation
{
    public interface INavigationService
    {
        ViewModelBase? CurrentViewModel { get; }
        event Action? CurrentViewModelChanged;
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    }
}