using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.Utils;

namespace HAISelenium.InternalActions
{
    internal class LoginActions
    {
        internal static void PerformLogin(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Performing login...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            try
            {
                string username = Utilities.GetEnvironmentVariableOrThrow("USERNAME");
                string password = Utilities.GetEnvironmentVariableOrThrow("PASSWORD");
                Console.WriteLine("[INFO] Retrieved username and password from environment variables.");

                IWebElement usernameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Username")));
                IWebElement passwordInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Password")));
                IWebElement submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("SubmitButton")));
                Console.WriteLine("[INFO] Login form elements located.");

                usernameInput.Clear();
                usernameInput.SendKeys(username);
                Console.WriteLine("[INFO] Entered username.");

                passwordInput.Clear();
                passwordInput.SendKeys(password);
                Console.WriteLine("[INFO] Entered password.");

                submitButton.Click();
                Console.WriteLine("[INFO] Submit button clicked.");

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".fixed-top.header-text")));
                Console.WriteLine("[SUCCESS] Login successful, header text found.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"[ERROR] Timeout while waiting for elements during login: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred during login: {ex.Message}");
                throw;
            }
        }
    }
}
