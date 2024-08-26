using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.StatusRequest;

namespace HAI_Selenium.Workflow.Steps.StatusRequest
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
                ClaimStatus ClaimStatus = Context.Get<ClaimStatus>("ClaimContainer");
                List<ClaimStatusLineItem> ClaimStatusLineItems = Context.Get<List<ClaimStatusLineItem>>("ClaimLineItems");
                List<ClaimsStatusWithLineItems> ClaimStatuses = Context.Get<List<ClaimsStatusWithLineItems>>("ClaimStatuses");
                ClaimsStatusWithLineItems claimsStatusWithLineItems = new(ClaimStatus, ClaimStatusLineItems);

                ClaimStatuses.Add(claimsStatusWithLineItems);

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

