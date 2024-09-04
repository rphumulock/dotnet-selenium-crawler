using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.StatusRequest;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.StatusRequest
{

    internal class ProcessClaimLineItemsAction(WorkflowContext context) : WorkflowStepBase(context)
    {

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Looking up claim...");

            IWebElement table = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("table[id*='exampleTable']")));
            WaitForTableData(driver, table);

            Log.Information("Claim line items grid loaded.");

            IWebElement tableBody = table.FindElement(By.CssSelector("tbody"));
            IList<IWebElement> tableRows = tableBody.FindElements(By.TagName("tr"));

            var claimsDataLineItems = tableRows
                .Select(tr => tr.FindElements(By.TagName("td"))
                    .Select(td => td.Text.Trim())
                    .ToList())
                .Where(cellTexts => cellTexts.Count > 9) // Ensure there are enough cells
                .Select(cellTexts => new ClaimStatusLineItem
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

            Context.Set("ClaimLineItems", claimsDataLineItems);

            Log.Information("[SUCCESS] Claim line items processed.");

            return Task.CompletedTask;
        }

        public void WaitForTableData(IWebDriver driver, IWebElement table)
        {
            WaitUntil(driver, drv =>
            {
                var cells = table.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });

            Log.Information("Modal opened.");
        }

    }
}
