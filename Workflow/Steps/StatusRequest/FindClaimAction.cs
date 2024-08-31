using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.StatusRequest;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.StatusRequest
{
    internal class FindClaimAction : WorkflowStepBase
    {

        protected ClaimStatusRequest ClaimStatusRequest { get; init; }

        internal FindClaimAction(WorkflowContext context, ClaimStatusRequest claimStatusRequest) : base(context)
        {
            ClaimStatusRequest = claimStatusRequest;
        }

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Looking up claim #{ClaimID}...", ClaimStatusRequest.ClaimID);

            IWebElement claimNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCS_ClaimNumber")));
            claimNumberInput.Click();
            claimNumberInput.SendKeys(Keys.Control + "a");
            claimNumberInput.SendKeys(Keys.Delete);
            claimNumberInput.SendKeys(ClaimStatusRequest.ClaimID);

            Log.Information("Claim ID entered.");

            IWebElement searchClaimsButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("btnClmsSearch")));
            searchClaimsButton.Click();

            Log.Information("[SUCCESS] Search initiated for claim.");

            return Task.CompletedTask;
        }
    }
}
