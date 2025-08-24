using System;
using System.ComponentModel;

namespace ShoukatSons.POS.Wpf.Services.Session
{
    /// <summary>
    /// Thread‐safe, bindable session contract.
    /// </summary>
    public interface ISessionService : INotifyPropertyChanged
    {
        event Action? SessionStarted;
        event Action? SessionEnded;

        string Username { get; }
        string Role { get; }
        bool   IsAuthenticated { get; }

        void Start(string username, string role);
        void Clear();
    }
}