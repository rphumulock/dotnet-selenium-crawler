using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.StatusRequest;
using HAI_Selenium.Workflow.Classes;


namespace HAI_Selenium.Workflow.Steps.StatusRequest
{

    internal class CreateClaimStatusAction(WorkflowContext context) : WorkflowStepBase(context)
    {

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Creating Claim Status...");

            ClaimStatus ClaimStatus = Context.Get<ClaimStatus>("ClaimContainer");
            List<ClaimStatusLineItem> ClaimStatusLineItems = Context.Get<List<ClaimStatusLineItem>>("ClaimLineItems");
            List<ClaimsStatusWithLineItems> ClaimStatuses = Context.Get<List<ClaimsStatusWithLineItems>>("ClaimStatuses");
            ClaimsStatusWithLineItems claimsStatusWithLineItems = new(ClaimStatus, ClaimStatusLineItems);

            ClaimStatuses.Add(claimsStatusWithLineItems);

            Context.Set("ClaimStatuses", ClaimStatuses);

            Log.Information("[SUCCESS] Claim Status created successfully.");

            return Task.CompletedTask;
        }

        public void WaitForTableData(IWebDriver driver, IWebElement table)
        {
            WaitUntil(driver, drv =>
            {
                var cells = table.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });

            Log.Information("Modal opened and table data loaded.");
        }
    }
}
