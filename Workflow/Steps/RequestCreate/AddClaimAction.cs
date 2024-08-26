using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.RequestCreate
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
                Console.WriteLine("[ACTION] Adding Claim...");

                var addButton = Context.Get<IWebElement>("AddButtonElement");
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(addButton));
                addButton.Click();

                Console.WriteLine("[INFO] Clicked 'Add' button.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");
                throw;
            }
        }
    }
}