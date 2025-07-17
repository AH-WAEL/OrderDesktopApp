using Microsoft.EntityFrameworkCore;
namespace desktopAPI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            // Initialize logging system
            Logging.Initialize();
            Logging.LogUserAction("Application", "Application started", "Desktop API application starting up");
            
            try
            {
                // Start with login form
                Logging.LogUserAction("Application", "Starting login process", "Showing Form4 for user authentication");
                Application.Run(new Form4());
            }
            catch (Exception ex)
            {
                Logging.LogError("Application", "Application error", ex, "Unhandled exception in main application loop");
            }
            finally
            {
                // Shutdown logging
                Logging.LogUserAction("Application", "Application shutting down", "Normal application exit");
                Logging.Shutdown();
            }
        }
    }
}