using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    internal abstract class NavigationAction : WorkflowStepBase
    {
        internal NavigationAction(WorkflowContext context) : base(context)
        {
            Context = context;
        }

        protected abstract string DropdownSelector { get; }

        protected abstract string LinkSelector { get; }

        protected abstract string DropdownName { get; }

        protected abstract string LinkName { get; }

        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Navigating to {DropdownName} -> {LinkName}...", DropdownName, LinkName);

            try
            {
                // Use the WaitUntil method from WorkflowStepBase to wait for the dropdown to be clickable
                IWebElement dropdownToggle = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(DropdownSelector)));
                dropdownToggle.Click();
                Log.Information("{DropdownName} dropdown clicked.", DropdownName);

                // Use the WaitUntil method from WorkflowStepBase to wait for the link to be visible
                IWebElement link = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(LinkSelector)));
                link.Click();
                Log.Information("{LinkName} link clicked.", LinkName);

                Log.Information("[SUCCESS] Navigating to {DropdownName} -> {LinkName}...", DropdownName, LinkName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
