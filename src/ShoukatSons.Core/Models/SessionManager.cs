namespace ShoukatSons.Core.Models
{
    /// <summary>
    /// Holds session-scoped state, like the current logged-in user.
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// The currently authenticated user. Null until someone logs in.
        /// </summary>
        public User? CurrentUser { get; set; }
    }
}