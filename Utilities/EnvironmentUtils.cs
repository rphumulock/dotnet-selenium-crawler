using dotenv.net;
using Serilog;
using System;

namespace HAI_Selenium.Utilities
{
    internal static class EnvironmentUtils
    {
        internal static string DbConnectionStringBuilder()
        {
            string host = GetEnvironmentVariableOrThrow("DB_HOST");
            string port = GetEnvironmentVariableOrThrow("DB_PORT");
            string database = GetEnvironmentVariableOrThrow("DB_NAME");
            string username = GetEnvironmentVariableOrThrow("DB_USER");
            string password = GetEnvironmentVariableOrThrow("DB_PASSWORD");

            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            return connectionString;
        }

        internal static void LoadEnvVariables()
        {
            Log.Information("[ACTION] Loading environment variables...");
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Log.Information("[SUCCESS] Environment variables loaded.");
        }

        internal static string GetChromeUserDataDir()
        {
            return GetEnvironmentVariableOrThrow("CHROME_USER_DATA_DIR");
        }

        internal static string GetChromeProfileDir()
        {
            return GetEnvironmentVariableOrThrow("CHROME_PROFILE_DIR");
        }

        internal static string GetEnvironmentVariableOrThrow(string key)
        {
            Log.Information("Attempting to retrieve from environment: {Action}", key);

            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                Log.Error("Environment variable '{Key}' is not set.", key);
                throw new InvalidOperationException($"Environment variable '{key}' is not set.");
            }

            Log.Information("Action retrieved from environment: {Action}", value);

            return value;
        }

        internal static void LogCurrentUserInfo()
        {
            Log.Information("[ACTION] Logging current user info...");

            var userName = GetEnvironmentVariableOrThrow("USERNAME");
            var userDomainName = Environment.UserDomainName;
            Log.Information("[INFO] Current User: {UserDomainName}\\{UserName}", userDomainName, userName);

            Log.Information("[SUCCESS] User info logged.");
        }
    }
}
