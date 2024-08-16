using dotenv.net;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using OpenQA.Selenium;
using HAISelenium.InternalClasses;
using OpenQA.Selenium.Support.UI;

namespace HAISelenium.Utils
{
    internal static class Utilities
    {
        internal static IWebDriver SetupDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = SetupChromeOptions();

            ChromeDriver driver = new ChromeDriver(options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            return driver;
        }

        internal static void LoadEnvVariables()
        {
            DotEnv.Load();
            Console.WriteLine("Environment variables loaded.");
        }

        internal static T LoadJsonFile<T>(string filePath)
        {
            try
            {
                // Read JSON from the file
                string json = File.ReadAllText(filePath);

                // Parse the JSON string into an object of type T
                T data = JsonConvert.DeserializeObject<T>(json);
                Console.WriteLine("JSON file loaded and parsed.");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading the JSON file: {ex.Message}");
                throw;
            }
        }

        internal static ChromeOptions SetupChromeOptions()
        {
            string userDataDir = GetEnvironmentVariableOrThrow("CHROME_USER_DATA_DIR");
            string profileDir = GetEnvironmentVariableOrThrow("CHROME_PROFILE_DIR");

            VerifyDirectoryExists(userDataDir);
            VerifyDirectoryExists(Path.Combine(userDataDir, profileDir));

            var options = new ChromeOptions();
            options.AddArgument("--allowed-ips=127.0.0.1,192.168.1.100");
            options.AddArgument($"--user-data-dir={userDataDir}");
            options.AddArgument($"--profile-directory={profileDir}");
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--enable-features=NewUsbBackend");

            Console.WriteLine("Chrome options set.");
            return options;
        }

        internal static void VerifyDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }
        }

        internal static string GetEnvironmentVariableOrThrow(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Environment variable '{key}' is not set.");
            }
            return value;
        }

        internal static void LogCurrentUserInfo()
        {
            string userName = GetEnvironmentVariableOrThrow("USERNAME");
            string userDomainName = Environment.UserDomainName;
            Console.WriteLine($"Current User: {userDomainName}\\{userName}");
        }

        internal static void Retry(Action action, int retries, string errorMessage)
        {
            int attempt = 0;
            while (attempt < retries)
            {
                try
                {
                    action();
                    return;
                }
                catch (NoSuchElementException ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: Element not found. {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (ElementClickInterceptedException ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: Element click intercepted. {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (WebDriverTimeoutException ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: Timeout. {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: {ex.Message}");
                    if (attempt >= retries) throw;
                }
            }
        }

        internal static List<List<ServiceDateData>> BatchServiceDates(List<ServiceDateData> claims, int batchSize)
        {
            List<List<ServiceDateData>> batches = new List<List<ServiceDateData>>();

            for (int i = 0; i < claims.Count; i += batchSize)
            {
                List<ServiceDateData> batch = claims.GetRange(i, Math.Min(batchSize, claims.Count - i));
                batches.Add(batch);
            }

            return batches;
        }

        internal static WebDriverWait CreateWebDriverWait(IWebDriver driver, int timeoutSeconds = 20)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        }

    }
}
