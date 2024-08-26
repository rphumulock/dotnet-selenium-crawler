using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.StatusRequest;

namespace HAI_Selenium.Workflow.Steps.StatusRequest
{
    internal class ProcessClaimHeaderAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal ProcessClaimHeaderAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine($"[ACTION] Processing Claim...");

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
                    .Select(cellTexts => new ClaimStatus
                    {
                        ClaimNumber = cellTexts[1],
                        NPI = cellTexts[2],
                        Member = cellTexts[3],
                        Provider = cellTexts[4],
                        Status = cellTexts[5],
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

                Context.Set("ClaimContainer", claimData);
                Context.Set("OpenClaimLineItemsElement", claimsTableRows[0]);

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

