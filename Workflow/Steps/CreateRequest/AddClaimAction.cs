using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class AddClaimAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal AddClaimAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            try
            {
                Log.Information("[ACTION] Adding Claim...");

                var addButton = Context.Get<IWebElement>("AddButtonElement");
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(addButton));
                addButton.Click();

                Log.Information("[SUCCESS] Clicked 'Add' button.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while processing form: {Message}", ex.Message);
                throw;
            }
        }
    }
}
