using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.InternalClasses;

namespace HAISelenium.InternalActions
{
    class ServiceRequestActions
    {
        internal static ServiceRequest SelectServiceRequestWithAuthNumber(IWebDriver driver, string serviceMonth)
        {
            Console.WriteLine("[ACTION] Starting lookup for Service Request Authorization number...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement servicesGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("servicesGrid")));
            Console.WriteLine("[INFO] Services grid found and loaded.");

            wait.Until(driver =>
            {
                var cells = servicesGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });
            Console.WriteLine("[INFO] Services grid cells loaded successfully.");

            var trElements = servicesGrid.FindElements(By.CssSelector("tbody tr:not(:first-child)"));
            Console.WriteLine($"[INFO] Found {trElements.Count} service request entries.");

            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();

            foreach (var tr in trElements)
            {
                List<string> cellTexts = new List<string>();
                var tdElements = tr.FindElements(By.CssSelector("td:not([style*='display: none'])"));

                foreach (var td in tdElements)
                {
                    try
                    {
                        var firstChild = td.FindElement(By.CssSelector(":first-child"));
                        if (firstChild != null)
                        {
                            cellTexts.Add(firstChild.Text.Trim());
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        continue;
                    }
                }

                if (cellTexts.Count > 0)
                {
                    serviceRequests.Add(new ServiceRequest
                    {
                        ID = cellTexts[1],
                        SRID = cellTexts[2],
                        SRAuth = cellTexts[3],
                        AuthApprov = cellTexts[4],
                        AuthStatus = cellTexts[5],
                        ProvSite = cellTexts[6],
                        Phone = cellTexts[10],
                        Procedure = cellTexts[11],
                        StartDate = cellTexts[12],
                        EndDate = cellTexts[13],
                        Units = cellTexts[14],
                        SubmissionDate = cellTexts[21],
                        ModifiedDate = cellTexts[25]
                    });
                }
            }

            Console.WriteLine($"[INFO] Extracted {serviceRequests.Count} service requests from the grid.");

            // Filter out only the "Approved" service requests
            var approvedRequests = serviceRequests.Where(request => request.AuthStatus == "Approved").ToList();
            Console.WriteLine($"[INFO] Found {approvedRequests.Count} approved service requests.");

            // Further filter by the serviceMonth
            var filteredRequests = approvedRequests
                .Where(request => request.StartDate.Split('/')[0] == serviceMonth)
                .ToList();
            Console.WriteLine($"[INFO] Found {filteredRequests.Count} service requests matching month: {serviceMonth}.");

            // Find the request with the latest day
            var latestRequest = filteredRequests
                .OrderByDescending(request => int.Parse(request.StartDate.Split('/')[1]))
                .FirstOrDefault();

            if (latestRequest != null)
            {
                Console.WriteLine("[SUCCESS] Latest approved service request found:");
                Console.WriteLine($"ID: {latestRequest.ID}");
                Console.WriteLine($"SRID: {latestRequest.SRID}");
                Console.WriteLine($"SRAuth: {latestRequest.SRAuth}");
                Console.WriteLine($"AuthApprov: {latestRequest.AuthApprov}");
                Console.WriteLine($"AuthStatus: {latestRequest.AuthStatus}");
                Console.WriteLine($"ProvSite: {latestRequest.ProvSite}");
                Console.WriteLine($"Phone: {latestRequest.Phone}");
                Console.WriteLine($"Procedure: {latestRequest.Procedure}");
                Console.WriteLine($"StartDate: {latestRequest.StartDate}");
                Console.WriteLine($"EndDate: {latestRequest.EndDate}");
                Console.WriteLine($"Units: {latestRequest.Units}");
                Console.WriteLine($"SubmissionDate: {latestRequest.SubmissionDate}");
                Console.WriteLine($"ModifiedDate: {latestRequest.ModifiedDate}");
            }
            else
            {
                Console.WriteLine($"[WARN] No approved service requests found for month: {serviceMonth}");
            }

            return latestRequest;
        }
    }
}
