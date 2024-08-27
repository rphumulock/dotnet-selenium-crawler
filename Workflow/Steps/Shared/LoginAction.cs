using OpenQA.Selenium;
using HAI_Selenium.Utilities;
using Serilog;
using SeleniumExtras.WaitHelpers;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    public class LoginAction : WorkflowStepBase
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Logging into the site...");

            try
            {
                // Get username and password from environment variables
                string username = EnvironmentUtils.GetEnvironmentVariableOrThrow("USERNAME");
                string password = EnvironmentUtils.GetEnvironmentVariableOrThrow("PASSWORD");
                Log.Information("Retrieved username and password from environment variables.");

                // Locate and fill in login form elements
                IWebElement usernameInput = WaitUntil(driver, ExpectedConditions.ElementIsVisible(By.Id("Username")));
                IWebElement passwordInput = WaitUntil(driver, ExpectedConditions.ElementIsVisible(By.Id("Password")));
                IWebElement submitButton = WaitUntil(driver, ExpectedConditions.ElementToBeClickable(By.Id("SubmitButton")));
                Log.Information("Login form elements located.");

                usernameInput.Click();
                usernameInput.SendKeys(Keys.Control + "a");
                usernameInput.SendKeys(Keys.Delete);
                usernameInput.SendKeys(username);
                Log.Information("Entered username.");

                passwordInput.Click();
                passwordInput.SendKeys(Keys.Control + "a");
                passwordInput.SendKeys(Keys.Delete);
                passwordInput.SendKeys(password);
                Log.Information("Entered password.");

                submitButton.Click();
                Log.Information("Submit button clicked.");

                // Wait for login confirmation
                WaitUntil(driver, ExpectedConditions.ElementIsVisible(By.CssSelector(".fixed-top.header-text")));
                Log.Information("[SUCCESS] Login successful, header text found.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during the login process: {Message}", ex.Message);
                throw;
            }
        }
    }
}
