using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class SelectServiceRequestAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Selecting Service Request Authorization Number...");

            IWebElement servicesGrid = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("servicesGrid")));
            Log.Information("Services grid found and loaded.");

            WaitUntil(driver, drv =>
            {
                var cells = servicesGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });
            Log.Information("Services grid cells loaded successfully.");

            // Extract service request entries
            var trElements = servicesGrid.FindElements(By.CssSelector("tbody tr:not(:first-child)"));
            Log.Information("Found {ServiceRequestCount} service request entries.", trElements.Count);

            List<IncedoServiceRequest> serviceRequests = new List<IncedoServiceRequest>();

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
                    serviceRequests.Add(new IncedoServiceRequest
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

            Log.Information("Extracted {ServiceRequestCount} service requests from the grid.", serviceRequests.Count);

            // Filter out only the "Approved" service requests
            var approvedRequests = serviceRequests.Where(request => request.AuthStatus == "Approved").ToList();
            Log.Information("Found {ApprovedServiceRequestCount} approved service requests.", approvedRequests.Count);

            // Further filter by the ServiceMonth
            var serviceMonth = Context.Get<string>("ServiceMonth");
            var filteredRequests = approvedRequests
                .Where(request => request.StartDate.Split('/')[0] == DateUtils.RemoveLeadingZero(serviceMonth))
                .ToList();
            Log.Information("Found {FilteredServiceRequestCount} service requests matching month: {ServiceMonth}.", filteredRequests.Count, serviceMonth);

            // Find the request with the latest day
            var latestRequest = filteredRequests
                .OrderByDescending(request => int.Parse(request.StartDate.Split('/')[1]))
                .FirstOrDefault();

            if (latestRequest != null)
            {
                Log.Information("[SUCCESS] Latest approved service request found: {LatestRequest}.", latestRequest.ToString());
            }
            else
            {
                Log.Warning("[WARN] No approved service requests found for month: {ServiceMonth}.", serviceMonth);
            }

            Context.Set("LatestServiceRequest", latestRequest);
        }
    }
}
