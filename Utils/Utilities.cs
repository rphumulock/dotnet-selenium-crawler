//using dotenv.net;
//using Newtonsoft.Json;
//using OpenQA.Selenium.Chrome;
//using WebDriverManager.DriverConfigs.Impl;
//using WebDriverManager;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Support.UI;
//using System.Globalization;
//using HAI_Selenium.InternalClasses.Invoice;
//using HAI_Selenium.InternalClasses.Request;

//namespace HAI_Selenium.Utils
//{
//    internal static class Utilities
//    {
//        internal static IWebDriver SetupDriver()
//        {
//            Console.WriteLine("[ACTION] Setting up WebDriver...");

//            new DriverManager().SetUpDriver(new ChromeConfig());
//            var options = SetupChromeOptions();
//            var driver = new ChromeDriver(options);
//            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

//            Console.WriteLine("[SUCCESS] WebDriver setup complete.");

//            return driver;
//        }

//        internal static void LoadEnvVariables()
//        {
//            Console.WriteLine("[ACTION] Loading environment variables...");
//            DotEnv.Load();
//            Console.WriteLine("[SUCCESS] Environment variables loaded.");
//        }

//        internal static T LoadJsonFile<T>(string filePath)
//        {
//            Console.WriteLine("[ACTION] Loading JSON file...");

//            try
//            {
//                var json = File.ReadAllText(filePath);
//                var data = JsonConvert.DeserializeObject<T>(json);
//                Console.WriteLine("[SUCCESS] JSON file loaded and parsed.");
//                return data;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[ERROR] Failed to load JSON file: {ex.Message}");
//                throw;
//            }
//        }

//        internal static ChromeOptions SetupChromeOptions()
//        {
//            var userDataDir = GetEnvironmentVariableOrThrow("CHROME_USER_DATA_DIR");
//            var profileDir = GetEnvironmentVariableOrThrow("CHROME_PROFILE_DIR");

//            VerifyDirectoryExists(userDataDir);
//            VerifyDirectoryExists(Path.Combine(userDataDir, profileDir));

//            var options = new ChromeOptions();
//            options.AddArgument($"--user-data-dir={userDataDir}");
//            options.AddArgument($"--profile-directory={profileDir}");
//            options.AddArgument("--remote-debugging-port=9222");
//            options.AddArgument("--no-sandbox");
//            options.AddArgument("--disable-extensions");
//            options.AddArgument("--enable-features=NewUsbBackend");

//            Console.WriteLine("[INFO] Chrome options configured.");
//            return options;
//        }

//        internal static void VerifyDirectoryExists(string path)
//        {
//            if (!Directory.Exists(path))
//            {
//                throw new DirectoryNotFoundException($"[ERROR] Directory not found: {path}");
//            }
//        }

//        internal static string GetEnvironmentVariableOrThrow(string key)
//        {
//            var value = Environment.GetEnvironmentVariable(key);
//            if (string.IsNullOrEmpty(value))
//            {
//                throw new InvalidOperationException($"[ERROR] Environment variable '{key}' is not set.");
//            }
//            return value;
//        }

//        internal static void LogCurrentUserInfo()
//        {
//            Console.WriteLine("[ACTION] Logging current user info...");

//            var userName = GetEnvironmentVariableOrThrow("USERNAME");
//            var userDomainName = Environment.UserDomainName;
//            Console.WriteLine($"[INFO] Current User: {userDomainName}\\{userName}");

//            Console.WriteLine("[SUCCESS] User info logged.");
//        }

//        public static string RemoveLeadingZero(string input)
//        {
//            if (input.StartsWith("0") && input.Length > 1)
//            {
//                return input.Substring(1);
//            }
//            return input;
//        }

//        internal static ServiceDateRequest FindLatestServiceDate(List<ServiceDateRequest> serviceDateRequests)
//        {
//            string[] formats = { "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy" };

//            return serviceDateRequests
//                .Select(serviceDateRequest =>
//                {
//                    if (!DateTime.TryParseExact(serviceDateRequest.ServiceDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
//                    {
//                        throw new InvalidOperationException($"Invalid date format: {serviceDateRequest.ServiceDate}");
//                    }
//                    return new { serviceDateRequest, parsedDate };
//                })
//                .OrderByDescending(x => x.parsedDate)
//                .First()
//                .serviceDateRequest;
//        }

//        internal static WebDriverWait CreateWebDriverWait(IWebDriver driver, int timeoutSeconds = 20)
//        {
//            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
//        }

//        public static string GetLastDayOfMonth(string monthString, string yearString)
//        {
//            if (!int.TryParse(monthString, out int month) || month is < 1 or > 12)
//            {
//                throw new ArgumentException("Invalid month format. Please enter a valid month as a number (e.g., '5' or '05').");
//            }

//            if (!int.TryParse(yearString, out int year))
//            {
//                throw new ArgumentException("Invalid year format. Please enter a valid year as a number (e.g., '2023').");
//            }

//            return DateTime.DaysInMonth(year, month).ToString();
//        }
//    }
//}
