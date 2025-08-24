using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShoukatSons.POS.Wpf.Services.Session
{
    /// <summary>
    /// Thread-safe, bindable session service that raises start/end events.
    /// </summary>
    public sealed class SessionService : ISessionService
    {
        private readonly object _lock = new();
        private string _username = string.Empty;
        private string _role     = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? SessionStarted;
        public event Action? SessionEnded;

        public string Username
        {
            get => _username;
            private set => SetProperty(ref _username, value, nameof(Username));
        }

        public string Role
        {
            get => _role;
            private set => SetProperty(ref _role, value, nameof(Role));
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_username);

        /// <summary>
        /// Starts a new session, setting user and role, raising PropertyChanged and SessionStarted.
        /// </summary>
        public void Start(string username, string role)
        {
            lock (_lock)
            {
                Username = username  ?? string.Empty;
                Role     = role      ?? string.Empty;
            }

            OnPropertyChanged(nameof(IsAuthenticated));
            SessionStarted?.Invoke();
        }

        /// <summary>
        /// Clears the session, resetting state and raising PropertyChanged and SessionEnded.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                Username = string.Empty;
                Role     = string.Empty;
            }

            OnPropertyChanged(nameof(IsAuthenticated));
            SessionEnded?.Invoke();
        }

        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}