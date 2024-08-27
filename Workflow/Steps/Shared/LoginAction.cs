using OpenQA.Selenium;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    public class LoginAction : WorkflowStepBase
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Logging into the site...");

            try
            {
                // Get username and password from environment variables
                string username = EnvironmentUtils.GetEnvironmentVariableOrThrow("USERNAME");
                string password = EnvironmentUtils.GetEnvironmentVariableOrThrow("PASSWORD");
                Console.WriteLine("[INFO] Retrieved username and password from environment variables.");

                // Locate and fill in login form elements
                IWebElement usernameInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Username")));
                IWebElement passwordInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Password")));
                IWebElement submitButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("SubmitButton")));
                Console.WriteLine("[INFO] Login form elements located.");

                usernameInput.Click();
                usernameInput.SendKeys(Keys.Control + "a");
                usernameInput.SendKeys(Keys.Delete);
                usernameInput.SendKeys(username);
                Console.WriteLine("[INFO] Entered username.");

                passwordInput.Click();
                passwordInput.SendKeys(Keys.Control + "a");
                passwordInput.SendKeys(Keys.Delete);
                passwordInput.SendKeys(password);
                Console.WriteLine("[INFO] Entered password.");

                submitButton.Click();
                Console.WriteLine("[INFO] Submit button clicked.");

                // Wait for login confirmation
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".fixed-top.header-text")));
                Console.WriteLine("[SUCCESS] Login successful, header text found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the login process: {ex.Message}");
                throw;
            }
        }
    }
}
