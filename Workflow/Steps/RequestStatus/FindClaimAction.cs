using HAI_Selenium.InternalClasses.Status;
using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.RequestStatus
{
    internal class FindClaimAction : WorkflowStepBase
    {
        protected ClaimRequest ClaimRequest { get; init; }

        internal FindClaimAction(ClaimRequest claimRequest)
        {
            ClaimRequest = claimRequest;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine($"[ACTION] Looking up claim #{ClaimRequest.ClaimID}...");

            try
            {
                IWebElement claimNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCS_ClaimNumber")));
                claimNumberInput.Click();
                claimNumberInput.SendKeys(Keys.Control + "a");
                claimNumberInput.SendKeys(Keys.Delete);
                claimNumberInput.SendKeys(ClaimRequest.ClaimID);

                Console.WriteLine("[INFO] Claim ID entered.");

                IWebElement searchClaimsButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("btnClmsSearch")));
                searchClaimsButton.Click();

                Console.WriteLine("[INFO] Search initiated for claim.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing service dates: {ex.Message}");
                throw;
            }
        }
    }
}