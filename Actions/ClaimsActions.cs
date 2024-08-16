using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace HAI_Selenium.Actions
{
    internal class ClaimsActions
    {
        internal static void AddClaim(IWebDriver driver)
        {
            Console.WriteLine($"Adding Claim ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement addButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
            addButton.Click();
            Console.WriteLine("Add Claim button clicked successfully.");
        }
    }
}