namespace ShoukatSons.UI.Services.Interfaces
{
    public interface IWindowService
    {
        /// <summary>
        /// Finds the Window whose DataContext is the supplied viewModel and closes it.
        /// </summary>
        void CloseWindow(object viewModel);
    }
}