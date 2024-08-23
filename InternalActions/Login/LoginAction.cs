using HAI_Selenium.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class LoginAction : WorkflowStepBase
{
    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine("[ACTION] Logging into the site...");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            // Get username and password from environment variables
            string username = Utilities.GetEnvironmentVariableOrThrow("USERNAME");
            string password = Utilities.GetEnvironmentVariableOrThrow("PASSWORD");
            Console.WriteLine("[INFO] Retrieved username and password from environment variables.");

            // Locate and fill in login form elements
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

            // Wait for login confirmation
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".fixed-top.header-text")));
            Console.WriteLine("[SUCCESS] Login successful, header text found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] An error occurred during the login process: {ex.Message}");
            throw;
        }
    }
}
