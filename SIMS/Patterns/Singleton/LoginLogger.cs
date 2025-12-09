using System;
using System.IO;

namespace SIMS.Patterns.Singleton
{
    public sealed class LoginLogger
    {
        private static LoginLogger? _instance;
        private static readonly object _lock = new object();
        private readonly string _logFilePath;

        // Private constructor to prevent external instantiation
        private LoginLogger()
        {
            // Log file stored in the application execution directory
            // In a real web app, this might be configured in appsettings.json
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            _logFilePath = Path.Combine(logDirectory, "login_attempts.log");
        }

        // Public static property to get the single instance
        public static LoginLogger Instance
        {
            get
            {
                // Double-check locking for thread safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoginLogger();
                        }
                    }
                }
                return _instance;
            }
        }

        public void LogLogin(string username, bool success, string ipAddress = "Unknown")
        {
            try
            {
                string status = success ? "SUCCESS" : "FAILED";
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | User: {username} | Status: {status} | IP: {ipAddress}";

                // Using lock to ensure thread-safe file writing
                lock (_lock)
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch (Exception)
            {
                // ideally logging shouldn't break the app, so we swallow exceptions 
            }
        }

        public string GetLogFilePath()
        {
            return _logFilePath;
        }
    }
}
