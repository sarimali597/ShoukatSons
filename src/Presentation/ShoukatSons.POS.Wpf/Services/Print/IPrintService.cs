namespace ShoukatSons.POS.Wpf.Services.Print
{
    /// <summary>
    /// Simple printing abstraction.
    /// </summary>
    public interface IPrintService
    {
        /// <summary>
        /// Print the given text (or send it to a printer).
        /// </summary>
        void Print(string content);
    }
}