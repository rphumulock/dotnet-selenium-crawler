using dotenv.net;

namespace HAI_Selenium.Utilities
{
    internal static class EnvironmentUtils
    {
        internal static void LoadEnvVariables()
        {
            Console.WriteLine("[ACTION] Loading environment variables...");
            DotEnv.Load();
            Console.WriteLine("[SUCCESS] Environment variables loaded.");
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
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"[ERROR] Environment variable '{key}' is not set.");
            }
            return value;
        }

        internal static void LogCurrentUserInfo()
        {
            Console.WriteLine("[ACTION] Logging current user info...");

            var userName = GetEnvironmentVariableOrThrow("USERNAME");
            var userDomainName = Environment.UserDomainName;
            Console.WriteLine($"[INFO] Current User: {userDomainName}\\{userName}");

            Console.WriteLine("[SUCCESS] User info logged.");
        }
    }
}
