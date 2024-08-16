using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.Utils;

namespace HAISelenium.Actions
{
    internal class LoginActions
    {
        internal static void PerformLogin(IWebDriver driver)
        {
            Console.WriteLine("Performing Login ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            string username = Utilities.GetEnvironmentVariableOrThrow("USERNAME");
            string password = Utilities.GetEnvironmentVariableOrThrow("PASSWORD");

            IWebElement usernameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Username")));
            IWebElement passwordInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Password")));
            IWebElement submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("SubmitButton")));

            usernameInput.Clear();
            usernameInput.SendKeys(username);

            passwordInput.Clear();
            passwordInput.SendKeys(password);

            submitButton.Click();

            Console.WriteLine("Login form submitted.");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".fixed-top.header-text")));
        }
    }
}
