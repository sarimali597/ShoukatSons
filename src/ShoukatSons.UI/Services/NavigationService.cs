using System;
using System.Collections.Generic;

namespace ShoukatSons.UI.Services
{
    public class NavigationService
    {
        private readonly Stack<object> _stack = new();

        public void Navigate(object viewModel)
        {
            _stack.Push(viewModel);
        }

        public object? GoBack()
        {
            if (_stack.Count > 1) _ = _stack.Pop();
            return _stack.Count > 0 ? _stack.Peek() : null;
        }

        public object? Current => _stack.Count > 0 ? _stack.Peek() : null;
    }
}