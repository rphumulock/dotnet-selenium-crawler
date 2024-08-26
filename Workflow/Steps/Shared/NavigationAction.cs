using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    internal abstract class NavigationAction : WorkflowStepBase
    {
        protected abstract string DropdownSelector { get; }
        protected abstract string LinkSelector { get; }
        protected abstract string DropdownName { get; }
        protected abstract string LinkName { get; }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine($"[ACTION] Navigating to {DropdownName} -> {LinkName}...");

            try
            {
                // Use the WaitUntil method from WorkflowStepBase to wait for the dropdown to be clickable
                IWebElement dropdownToggle = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(DropdownSelector)));
                dropdownToggle.Click();
                Console.WriteLine($"[INFO] {DropdownName} dropdown clicked.");

                // Use the WaitUntil method from WorkflowStepBase to wait for the link to be visible
                IWebElement link = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(LinkSelector)));
                link.Click();
                Console.WriteLine($"[INFO] {LinkName} link clicked.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"[ERROR] Timeout while navigating to {DropdownName} -> {LinkName}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while navigating to {DropdownName} -> {LinkName}: {ex.Message}");
                throw;
            }
        }
    }
}
