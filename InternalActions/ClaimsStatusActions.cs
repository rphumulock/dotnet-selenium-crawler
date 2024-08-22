using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAI_Selenium.InternalClasses;
using HAI_Selenium.Utils;

namespace HAI_Selenium.InternalActions
{
    class ClaimsStatusActions
    {
        internal static void CheckClaimsStatus(IWebDriver driver, StatusInvoice invoice)
        {
            Console.WriteLine("[ACTION] Starting patient claims status check...");

            Utilities.Retry(() => NavigationActions.NavigateToClaimsStatus(driver), 3, "[WARNING] Failed to navigate to Claims Status page. Retrying...");

            var claimsList = invoice.Claims.Select((claim, index) => new { claim, index }).ToList();
            List<ClaimDetails> allClaimsDetails = new List<ClaimDetails>();

            foreach (var indexItem in claimsList)
            {
                Console.WriteLine($"[ACTION] Processing claim for batch #{indexItem.claim.ClaimID}...");
                Utilities.Retry(() => FindClaim(driver, indexItem.claim), 3, $"[WARNING] Failed to find claim #{indexItem.claim.ClaimID}. Retrying...");

                ClaimDetails claimDetails = Utilities.Retry(() => ProcessClaimStatus(driver, indexItem.claim), 3, $"[WARNING] Failed to process status for claim #{indexItem.claim.ClaimID}. Retrying...");

                allClaimsDetails.Add(claimDetails);

                //Console.WriteLine($"[INFO] Processed claim #{indexItem.claim.ClaimID}. Claim Details: {claimDetails}");
            }

            Console.WriteLine("[SUCCESS] All claims processed successfully.");

            // Loop through and print each ClaimDetails object
            Console.WriteLine("[INFO] Printing all claim details...");
            foreach (var claimDetails in allClaimsDetails)
            {
                Console.WriteLine("---------------------\n");
                Console.WriteLine(claimDetails.ToString());
                Console.WriteLine("---------------------\n");
            }
        }

        internal static void FindClaim(IWebDriver driver, Claim claim)
        {
            Console.WriteLine($"[ACTION] Looking up claim #{claim.ClaimID}...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement claimNumberInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCS_ClaimNumber")));
            claimNumberInput.Click();
            claimNumberInput.SendKeys(Keys.Control + "a");
            claimNumberInput.SendKeys(Keys.Delete);
            claimNumberInput.SendKeys(claim.ClaimID);
            Console.WriteLine("[INFO] Claim ID entered.");

            IWebElement searchClaimsButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("btnClmsSearch")));
            searchClaimsButton.Click();
            Console.WriteLine("[INFO] Search initiated for claim.");
        }

        internal static ClaimDetails ProcessClaimStatus(IWebDriver driver, Claim claim)
        {
            Console.WriteLine($"[ACTION] Processing status for claim #{claim.ClaimID}...");

            ClaimData claimData = ProcessClaimHeader(driver);
            List<ClaimLineItem> claimLineItems = ProcessClaimLineItems(driver);
            ClaimDetails claimDetails = new ClaimDetails(claimData, claimLineItems);

            return claimDetails;
        }

        internal static ClaimData ProcessClaimHeader(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement claimsGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("claimsGrids")));
            wait.Until(driver =>
            {
                var cells = claimsGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });
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

            Console.WriteLine("[INFO] Claim header processed.");

            // Open line items for the first row
            OpenLineItems(claimsTableRows[0]);

            return claimData;
        }

        internal static void OpenLineItems(IWebElement openLineItemsElement)
        {
            Console.WriteLine("[ACTION] Expanding claim line items...");

            IWebElement openLineItemsButton = openLineItemsElement.FindElement(By.CssSelector("a[id*='claims-link']"));
            openLineItemsButton.Click();
            Console.WriteLine("[INFO] Claim line items expanded.");
        }

        private static List<ClaimLineItem> ProcessClaimLineItems(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement table = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("table[id*='exampleTable']")));
            wait.Until(driver =>
            {
                var cells = table.FindElements(By.CssSelector("table tbody td > :first-child"));
                return cells.Count > 0;
            });
            Console.WriteLine("[INFO] Claim line items grid loaded.");

            IWebElement tableBody = table.FindElement(By.CssSelector("tbody"));
            IList<IWebElement> tableRows = tableBody.FindElements(By.TagName("tr"));

            var claimsDataLineItems = tableRows
                .Select(tr => tr.FindElements(By.TagName("td"))
                    .Select(td => td.Text.Trim())
                    .ToList())
                .Where(cellTexts => cellTexts.Count > 9) // Ensure there are enough cells
                .Select(cellTexts => new ClaimLineItem
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

            Console.WriteLine("[INFO] Claim line items processed.");

            return claimsDataLineItems;
        }
    }
}
