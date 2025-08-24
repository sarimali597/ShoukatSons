using ShoukatSons.Core.Models;

namespace ShoukatSons.Services
{
    public class SessionManager
    {
        public User CurrentUser { get; set; } = new User();

        public bool IsUserLoggedIn => CurrentUser != null && CurrentUser.Id != 0;

        public void Login(User user)
        {
            CurrentUser = user ?? new User();
        }

        public void Logout()
        {
            CurrentUser = new User();
        }
    }
}