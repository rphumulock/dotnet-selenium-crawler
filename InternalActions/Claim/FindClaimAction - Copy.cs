using HAI_Selenium.InternalClasses.Status;
using OpenQA.Selenium;
using System.Security.Claims;

namespace HAI_Selenium.InternalActions
{
    internal class ProcessClaimHeader : WorkflowStepBase
    {
        protected ClaimRequest ClaimRequest { get; init; }

        internal ProcessClaimHeader(ClaimRequest claimRequest)
        {
            ClaimRequest = claimRequest;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine($"[ACTION] Looking up claim #{ClaimRequest.ClaimID}...");

            try
            {
                IWebElement claimsGrid = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("claimsGrid")));
                WaitForTableData(driver, claimsGrid);

                Console.WriteLine("[INFO] Claims grid loaded.");

                IWebElement tbodyElement = claimsGrid.FindElement(By.CssSelector(":scope > tbody"));
                IList<IWebElement> claimsTableRows = tbodyElement.FindElements(By.CssSelector(":scope > tr"));

                var claimData = claimsTableRows
                    .Select(tr => tr.FindElements(By.CssSelector("td")).Select(td => td.Text.Trim()).ToList())
                    .Where(cellTexts => cellTexts.Count > 10) // Ensure there are enough cells
                    .Select(cellTexts => new ClaimData
                    {
                        ClaimNumber = cellTexts[1],
                        NPI = cellTexts[2],
                        Member = cellTexts[3],
                        Provider = cellTexts[4],
                        ClaimStatus = cellTexts[5],
                        DateReceived = cellTexts[6],
                        TotalCharge = cellTexts[7],
                        TotalApproved = cellTexts[8],
                        ServiceDates = cellTexts[9],
                        BatchNumber = cellTexts[10]
                    })
                    .FirstOrDefault(); // Get the first valid claim or null if none found

                if (claimData == null)
                {
                    throw new Exception("[ERROR] No valid claim data found.");
                }

                Context.Set("Claim" + ClaimRequest.ClaimID, claimData);

                Console.WriteLine("[INFO] Claim header processed.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing service dates: {ex.Message}");
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

