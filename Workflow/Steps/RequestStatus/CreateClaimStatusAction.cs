using HAI_Selenium.InternalClasses.Status;
using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.RequestStatus
{
    internal class CreateClaimStatusAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal CreateClaimStatusAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine($"[ACTION] Creating Claim Status...");

            try
            {
                ClaimContainer ClaimContainer = Context.Get<ClaimContainer>("ClaimContainer");
                List<ClaimLineItemContainer> ClaimLineItems = Context.Get<List<ClaimLineItemContainer>>("ClaimLineItems");
                List<ClaimStatusContainer> ClaimStatuses = Context.Get<List<ClaimStatusContainer>>("ClaimStatuses");

                ClaimStatusContainer claimStatusContainer = new(ClaimContainer, ClaimLineItems);
                ClaimStatuses.Add(claimStatusContainer);

                Context.Set("ClaimStatuses", ClaimStatuses);

                Console.WriteLine("[INFO] Claim Status created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while creating Claim Status: {ex.Message}");
                throw;
            }
        }

        public void WaitForTableData(IWebDriver driver, IWebElement table)
        {
            WaitUntil(driver, driver =>
            {
                var cells = table.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });

            Console.WriteLine("[INFO] Modal opened.");
        }
    }
}

