using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.RequestCreate
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
                Console.WriteLine("[ACTION] Cancelling Claim...");

                var cancelButton = Context.Get<IWebElement>("CancelButtonElement");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", cancelButton);
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(cancelButton));
                cancelButton.Click();

                Console.WriteLine("[INFO] Clicked 'Cancel' button.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");
                throw;
            }
        }
    }
}
