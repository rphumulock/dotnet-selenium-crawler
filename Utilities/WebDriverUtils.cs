using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace HAI_Selenium.Utilities
{
    internal static class WebDriverUtils
    {
        internal static IWebDriver SetupDriver()
        {
            Console.WriteLine("[ACTION] Setting up WebDriver...");

            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = SetupChromeOptions();
            var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

            Console.WriteLine("[SUCCESS] WebDriver setup complete.");
            return driver;
        }

        private static ChromeOptions SetupChromeOptions()
        {
            string userDataDir = EnvironmentUtils.GetChromeUserDataDir();
            string profileDir = EnvironmentUtils.GetChromeProfileDir();

            var options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={userDataDir}");
            options.AddArgument($"--profile-directory={profileDir}");
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--enable-features=NewUsbBackend");

            Console.WriteLine("[INFO] Chrome options configured.");
            return options;
        }

        internal static class WebDriverUtilities
        {
            internal static WebDriverWait CreateWebDriverWait(IWebDriver driver, int timeoutSeconds = 20)
            {
                return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            }
        }
    }

}
