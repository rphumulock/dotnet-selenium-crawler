using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

public abstract class NavigationAction : WorkflowStepBase
{
    protected abstract string DropdownSelector { get; }
    protected abstract string LinkSelector { get; }
    protected abstract string DropdownName { get; }
    protected abstract string LinkName { get; }

    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine($"[ACTION] Navigating to {DropdownName} -> {LinkName}...");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            IWebElement dropdownToggle = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(DropdownSelector)));
            dropdownToggle.Click();
            Console.WriteLine($"[INFO] {DropdownName} dropdown clicked.");

            IWebElement link = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(LinkSelector)));
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
