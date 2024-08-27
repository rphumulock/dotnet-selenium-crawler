using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class CancelClaimAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal CancelClaimAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            try
            {
                Log.Information("[ACTION] Cancelling Claim...");

                var cancelButton = Context.Get<IWebElement>("CancelButtonElement");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", cancelButton);
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(cancelButton));
                cancelButton.Click();

                Log.Information("[SUCCESS] Clicked 'Cancel' button.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while processing form: {Message}", ex.Message);
                throw;
            }
        }
    }
}
