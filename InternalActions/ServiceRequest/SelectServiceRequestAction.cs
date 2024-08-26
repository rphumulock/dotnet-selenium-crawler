using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.Utils;

internal class SelectServiceRequestAction : WorkflowStepBase
{
    protected WorkflowContext Context { get; init; }

    internal SelectServiceRequestAction(WorkflowContext context)
    {
        Context = context;
    }

    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine("[ACTION] Selecting Service Request Authorization Number...");

        try
        {
            IWebElement servicesGrid = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("servicesGrid")));
            Console.WriteLine("[INFO] Services grid found and loaded.");

            WaitUntil(driver, drv =>
            {
                var cells = servicesGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });
            Console.WriteLine("[INFO] Services grid cells loaded successfully.");

            // Extract service request entries
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

            // Further filter by the ServiceMonth
            var filteredRequests = approvedRequests
                .Where(request => request.StartDate.Split('/')[0] == DateUtils.RemoveLeadingZero(Context.Get<string>("ServiceMonth")))
                .ToList();
            Console.WriteLine($"[INFO] Found {filteredRequests.Count} service requests matching month: {Context.Get<string>("ServiceMonth")}.");

            // Find the request with the latest day
            var latestRequest = filteredRequests
                .OrderByDescending(request => int.Parse(request.StartDate.Split('/')[1]))
                .FirstOrDefault();

            if (latestRequest != null)
            {
                Console.WriteLine($"[SUCCESS] Latest approved service request found: {latestRequest.ToString()}");
            }
            else
            {
                Console.WriteLine($"[WARN] No approved service requests found for month: {Context.Get<string>("ServiceMonth")}");
            }

            // Store the service request in the context for later use
            Context.Set("ServiceRequest", latestRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] An unexpected error occurred while selecting service request: {ex.Message}");
            throw;
        }
    }
}
