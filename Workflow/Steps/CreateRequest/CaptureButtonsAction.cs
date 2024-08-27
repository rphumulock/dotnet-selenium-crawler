using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class CaptureButtonsAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal CaptureButtonsAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            try
            {
                Log.Information("[ACTION] Capturing Buttons...");

                var addButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
                var cancelButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran3")));
                Context.Set("AddButtonElement", addButton);
                Context.Set("CancelButtonElement", cancelButton);

                Log.Information("[SUCCESS] Captured 'Add' and 'Cancel' buttons.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while capturing buttons: {Message}", ex.Message);
                throw;
            }
        }
    }
}
