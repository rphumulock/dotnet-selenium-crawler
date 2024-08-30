using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class CancelClaimAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Cancelling Claim...");

            var cancelButton = Context.Get<IWebElement>("CancelButtonElement");
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", cancelButton);
            WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(cancelButton));
            cancelButton.Click();

            Log.Information("[SUCCESS] Clicked 'Cancel' button.");

            return Task.CompletedTask;
        }
    }
}
