using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.InternalClasses;

namespace HAISelenium.Actions
{
    class ServiceRequestActions
    {
        internal static string FindServiceRequestAuthorizationNumber(IWebDriver driver, string serviceMonth)
        {
            Console.WriteLine($"Looking up Service Request Authorization number ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement servicesGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("servicesGrid")));
            wait.Until(driver =>
            {
                var cells = servicesGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });

            var trElements = servicesGrid.FindElements(By.CssSelector("tbody tr:not(:first-child)"));

            List<List<string>> rowData = new List<List<string>>();
            for (int i = 0; i < trElements.Count; i++)
            {
                var tr = trElements[i];
                List<string> cellTexts = new List<string>();
                var tdElements = tr.FindElements(By.CssSelector("td:not([style*='display: none'])"));
                foreach (var td in tdElements)
                {
                    IWebElement firstChild = null;
                    try
                    {
                        firstChild = td.FindElement(By.CssSelector(":first-child"));
                    }
                    catch (NoSuchElementException)
                    {
                        continue;
                    }

                    if (firstChild != null)
                    {
                        cellTexts.Add(firstChild.Text.Trim());
                    }
                }
                rowData.Add(cellTexts);
            }

            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();
            foreach (var row in rowData)
            {
                ServiceRequest serviceRequest = new ServiceRequest
                {
                    ID = row[1],
                    SRID = row[2],
                    SRAuth = row[3],
                    AuthApprov = row[4],
                    AuthStatus = row[5],
                    ProvSite = row[6],
                    Phone = row[10],
                    Procedure = row[11],
                    StartDate = row[12],
                    EndDate = row[13],
                    Units = row[14],
                    SubmissionDate = row[21],
                    ModifiedDate = row[25]
                };

                serviceRequests.Add(serviceRequest);
            }

            // Filter out only the "Approved" service requests
            var approvedRequests = serviceRequests.Where(request => request.AuthStatus == "Approved").ToList();

            // Further filter by the serviceMonth
            var filteredRequests = approvedRequests
                .Where(request => request.StartDate.Split('/')[0] == serviceMonth)
                .ToList();

            // Find the request with the latest day
            var latestRequest = filteredRequests
                .OrderByDescending(request => int.Parse(request.StartDate.Split('/')[1]))
                .FirstOrDefault();

            if (latestRequest != null)
            {
                // Print the latest request
                Console.WriteLine($"\n\nLatest Approved Service Request for Month: {serviceMonth}");
                Console.WriteLine("---------------------------------------------");
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
                Console.WriteLine("---------------------------------------------");
            }
            else
            {
                Console.WriteLine($"No approved service requests found for month: {serviceMonth}");
            }

            return latestRequest?.SRAuth;
        }
    }
}
