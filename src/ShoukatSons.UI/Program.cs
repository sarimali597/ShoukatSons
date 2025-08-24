using System;
using System.Windows;

namespace ShoukatSons.UI
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Instantiate the Application and run
            var app = new App();
            app.Run(); // triggers OnStartup()
        }
    }
}