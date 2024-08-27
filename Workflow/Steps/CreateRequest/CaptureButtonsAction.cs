using HAI_Selenium.Utilities;
using OpenQA.Selenium;

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
                Console.WriteLine("[ACTION] Capturing Buttons...");

                var addButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
                var cancelButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran3")));
                Context.Set("AddButtonElement", addButton);
                Context.Set("CancelButtonElement", cancelButton);

                Console.WriteLine("[INFO] Captured 'Add' and 'Cancel' buttons.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");

                throw new RecoverableError("A non-recoverable error occurred.", ex);
            }
        }
    }
}