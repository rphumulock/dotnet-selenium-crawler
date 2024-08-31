using HAI_Selenium.Workflow.Classes;
using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Steps.StatusRequest
{

    internal class OpenClaimsLineItemsAction(WorkflowContext context) : WorkflowStepBase(context)
    {

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Expanding claim line items...");

            IWebElement openClaimLineItemsElement = Context.Get<IWebElement>("OpenClaimLineItemsElement");
            IWebElement openLineItemsButton = openClaimLineItemsElement.FindElement(By.CssSelector("a[id*='claims-link']"));
            openLineItemsButton.Click();

            Log.Information("[SUCCESS] Claim line items expanded.");

            return Task.CompletedTask;

        }
    }
}
