using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.Shared
{

    internal abstract class NavigationAction : WorkflowStepBase
    {

        internal NavigationAction(WorkflowContext context) : base(context) { }

        protected abstract string DropdownSelector { get; }

        protected abstract string LinkSelector { get; }

        protected abstract string DropdownName { get; }

        protected abstract string LinkName { get; }

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Navigating to {DropdownName} -> {LinkName}...", DropdownName, LinkName);

            try
            {
                IWebElement dropdownToggle = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(DropdownSelector)));
                dropdownToggle.Click();
                Log.Information("{DropdownName} dropdown clicked.", DropdownName);

                IWebElement link = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(LinkSelector)));
                link.Click();
                Log.Information("{LinkName} link clicked.", LinkName);

                Log.Information("[SUCCESS] Navigating to {DropdownName} -> {LinkName}...", DropdownName, LinkName);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
