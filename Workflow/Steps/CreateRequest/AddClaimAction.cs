using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class AddClaimAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Adding Claim...");

            var addButton = Context.Get<IWebElement>("AddButtonElement");
            WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(addButton));
            addButton.Click();

            Log.Information("[SUCCESS] Clicked 'Add' button.");

            return Task.CompletedTask;
        }
    }
}
