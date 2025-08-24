using System;
using System.Windows.Input;

namespace ShoukatSons.POS.Wpf.Common
{
    public class DelegateCommand : ICommand
    {
        private readonly Action        _execute;
        private readonly Func<bool>?   _canExecute;

        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute    = execute    ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Auto-hook into CommandManager so WPF will RequerySuggested for you
        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => 
            _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => 
            _execute();

        // Call this if your viewmodel decides manually to re-evaluate CanExecute
        public void RaiseCanExecuteChanged() =>
            CommandManager.InvalidateRequerySuggested();
    }
}