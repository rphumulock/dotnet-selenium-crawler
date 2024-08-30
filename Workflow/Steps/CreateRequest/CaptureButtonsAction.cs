
using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class CaptureButtonsAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Capturing Buttons...");

            var addButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
            var cancelButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran3")));
            Context.Set("AddButtonElement", addButton);
            Context.Set("CancelButtonElement", cancelButton);

            Log.Information("[SUCCESS] Captured 'Add' and 'Cancel' buttons.");

            return Task.CompletedTask;
        }
    }
}
