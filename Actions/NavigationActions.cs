using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.Utils;

namespace HAISelenium.Actions
{
    internal class NavigationActions
    {
        internal static void NavigateTo(IWebDriver driver, string dropdownSelector, string linkSelector, string dropdownName, string linkName)
        {
            Console.WriteLine($"Navigating to {dropdownName} {linkName} ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement dropdownToggle = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(dropdownSelector)));
            dropdownToggle.Click();
            Console.WriteLine($"{dropdownName} dropdown clicked.");

            IWebElement link = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(linkSelector)));
            link.Click();
            Console.WriteLine($"{linkName} link clicked.");
        }

        internal static void NavigateToSite(IWebDriver driver)
        {
            Console.WriteLine("Navigating to Site ...");

            string url = Utilities.GetEnvironmentVariableOrThrow("URL");
            driver.Navigate().GoToUrl(url);

            Console.WriteLine($"Navigated to {url}");
            Console.WriteLine($"Title: {driver.Title}");
            Console.WriteLine($"URL: {driver.Url}");
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
    }
}
