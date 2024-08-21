using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAI_Selenium.Utils;

namespace HAI_Selenium.InternalActions
{
    internal class NavigationActions
    {
        internal static void NavigateTo(IWebDriver driver, string dropdownSelector, string linkSelector, string dropdownName, string linkName)
        {
            Console.WriteLine($"[ACTION] Navigating to {dropdownName} -> {linkName}...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            try
            {
                IWebElement dropdownToggle = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(dropdownSelector)));
                dropdownToggle.Click();
                Console.WriteLine($"[INFO] {dropdownName} dropdown clicked.");

                IWebElement link = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(linkSelector)));
                link.Click();
                Console.WriteLine($"[INFO] {linkName} link clicked.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"[ERROR] Timeout while navigating to {dropdownName} -> {linkName}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while navigating to {dropdownName} -> {linkName}: {ex.Message}");
                throw;
            }
        }

        internal static void NavigateToSite(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Navigating to Site...");

            try
            {
                string url = Utilities.GetEnvironmentVariableOrThrow("URL");
                driver.Navigate().GoToUrl(url);

                Console.WriteLine($"[SUCCESS] Navigated to {url}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to navigate to site: {ex.Message}");
                throw;
            }
        }

        internal static void NavigateToMembershipSearch(IWebDriver driver)
        {
            NavigateTo(driver, "a[data-udfname='Membership']", "a[data-udfname='Search']", "Membership", "Search");
        }

        internal static void NavigateToAuthorizationRequests(IWebDriver driver)
        {
            NavigateTo(driver, "a[data-udfname='Authorization']", "a[data-udfname='Requests']", "Authorization", "Requests");
        }

        internal static void NavigateToAddClaims(IWebDriver driver)
        {
            NavigateTo(driver, "a[data-udfname='Claims']", "a[data-udfname='Add Claim']", "Claims", "Add Claim");
        }

        internal static void NavigateToClaimsStatus(IWebDriver driver)
        {
            NavigateTo(driver, "a[data-udfname='Claims']", "a[data-udfname='Claim Status']", "Claims", "Claim Status");
        }
    }
}
