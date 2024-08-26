using HAI_Selenium.InternalClasses.Status;
using OpenQA.Selenium;

namespace HAI_Selenium.InternalActions
{
    internal class OpenClaimsLineItemsAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal OpenClaimsLineItemsAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            try
            {
                Console.WriteLine("[ACTION] Expanding claim line items...");

                var openClaimLineItemsButton = Context.Get<IWebElement>("OpenClaimLineItemsButton");
                IWebElement openLineItemsButton = openClaimLineItemsButton.FindElement(By.CssSelector("a[id*='claims-link']"));
                openLineItemsButton.Click();

                Console.WriteLine("[INFO] Claim line items expanded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");
                throw;
            }
        }
    }
}
