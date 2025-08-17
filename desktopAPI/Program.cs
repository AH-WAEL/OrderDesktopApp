using desktopAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace desktopAPI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {

            AllocConsole();
            ApplicationConfiguration.Initialize();

            // Initialize logging system
            Logging.Initialize();
            Logging.LogUserAction("Application", "Application started", "Desktop API application starting up");

            try
            {
                ApplicationConfiguration.Initialize();                
                // Just run Form4 as the main application
                Application.Run(new Login());
            }
            catch (Exception ex)
            {
                Logging.LogError("Application", "Application error", ex, "Unhandled exception in main application loop");
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Application Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Clean up authentication tokens
                AuthApiService.ClearTokens();

                // Shutdown logging
                Logging.LogUserAction("Application", "Application shutting down", "Normal application exit");
                Logging.Shutdown();

                FreeConsole();
            }
        }
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
    }
}