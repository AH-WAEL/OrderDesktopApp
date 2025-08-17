using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.IO;

namespace desktopAPI
{
    public static class Logging
    {
        private static ILogger _logger;
        private static string _currentUsername = "Unknown";

        public static void Initialize()
        {
            var logDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Logs"));
            Directory.CreateDirectory(logDirectory);
            // Configure Serilog with structured logging
            _logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "user-actions-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [Username: {Username}] [Category: {Category}] [Action: {Action}] [Details: {Details}]{NewLine}{Exception}")
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [Username: {Username}] [Category: {Category}] [Action: {Action}] [Details: {Details}]{NewLine}")
                .CreateLogger();

            LogUserAction("Application", "Application started", "System initialization");
        }

        public static void SetCurrentUsername(string username)
        {
            _currentUsername = string.IsNullOrWhiteSpace(username) ? "Unknown" : username;
            LogUserAction("Authentication", "User logged in", $"Username set to: {_currentUsername}");
        }

        public static void LogUserAction(string category, string action, string details = "")
        {
            if (string.IsNullOrEmpty(details))
            {
                details = "No additional details";
            }

            _logger?.ForContext("Username", _currentUsername)
                .ForContext("Category", category)
                .ForContext("Action", action)
                .ForContext("Details", details)
                .Information("User action logged");
        }

        public static void LogError(string category, string action, Exception ex, string details = "")
        {
            if (string.IsNullOrEmpty(details))
            {
                details = ex?.Message ?? "No additional details";
            }

            _logger?.ForContext("Username", _currentUsername)
                .ForContext("Category", category)
                .ForContext("Action", action)
                .ForContext("Details", details)
                .Error(ex, "Error occurred");
        }

        public static void LogWarning(string category, string action, string message, string details = "")
        {
            if (string.IsNullOrEmpty(details))
            {
                details = message;
            }

            _logger?.ForContext("Username", _currentUsername)
                .ForContext("Category", category)
                .ForContext("Action", action)
                .ForContext("Details", details)
                .Warning("Warning logged");
        }

        public static void Shutdown()
        {
            LogUserAction("Application", "Application shutting down", "Normal application exit");
            Log.CloseAndFlush();
        }
    }
}