using HAI_Selenium.InternalClasses.Status;
using OpenQA.Selenium;

namespace HAI_Selenium.InternalActions
{
    internal class ProcessClaimLineItemsAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        internal ProcessClaimLineItemsAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine($"[ACTION] Looking up claim...");

            try
            {
                IWebElement table = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("table[id*='exampleTable']")));
                WaitForTableData(driver, table);

                Console.WriteLine("[INFO] Claim line items grid loaded.");

                IWebElement tableBody = table.FindElement(By.CssSelector("tbody"));
                IList<IWebElement> tableRows = tableBody.FindElements(By.TagName("tr"));

                var claimsDataLineItems = tableRows
                    .Select(tr => tr.FindElements(By.TagName("td"))
                        .Select(td => td.Text.Trim())
                        .ToList())
                    .Where(cellTexts => cellTexts.Count > 9) // Ensure there are enough cells
                    .Select(cellTexts => new ClaimLineItemContainer
                    {
                        LineItemControlNumber = cellTexts[0],
                        ServiceDates = cellTexts[1],
                        Service = cellTexts[2],
                        ServiceProcedureModifiers = cellTexts[3],
                        ServiceStatus = cellTexts[4],
                        Charge = cellTexts[5],
                        Approved = cellTexts[6],
                        Units = cellTexts[7],
                        ExceptionAdjudicationReason = cellTexts[8],
                        ReAdjudicationReason = cellTexts[9],
                    })
                    .ToList();

                Context.Set<List<ClaimLineItemContainer>>("ClaimLineItems", claimsDataLineItems);

                Console.WriteLine("[INFO] Claim line items processed.");
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

