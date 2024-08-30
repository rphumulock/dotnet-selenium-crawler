using OpenQA.Selenium;
using System.Globalization;
using Serilog;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    public class ValidateCreateRequestAction(WorkflowContext context) : WorkflowStepBase(context)
    {

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Validating service dates month ...");

            InvoiceRequest createClaimsRequest = Context.Get<InvoiceRequest>("InvoiceRequest");
            ICollection<ServiceDateRequest> serviceDateRequests = Context.Get<ICollection<ServiceDateRequest>>("ServiceDateRequests");

            if (createClaimsRequest.ServiceDateRequests == null || createClaimsRequest.ServiceDateRequests.Count == 0)
            {
                throw new ArgumentNullException("ServiceDateRequests cannot be null or empty.");
            }

            string serviceMonth = null;
            DateTime currentDate = DateTime.Today;
            foreach (var serviceDateRequest in createClaimsRequest.ServiceDateRequests)
            {
                if (!DateTime.TryParseExact(serviceDateRequest.ServiceDate, new[] { "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy" },
                                            null, DateTimeStyles.None, out DateTime parsedDate))
                {
                    throw new InvalidOperationException($"Invalid date format: {serviceDateRequest.ServiceDate}");
                }

                string currentMonth = parsedDate.Month.ToString("D2");

                if (parsedDate.Date == currentDate)
                {
                    throw new InvalidOperationException($"Service date {serviceDateRequest.ServiceDate} cannot be today's date.");
                }

                if (serviceMonth == null)
                {
                    serviceMonth = currentMonth;
                }
                else if (currentMonth != serviceMonth)
                {
                    throw new InvalidOperationException($"Mismatch found: expected month {serviceMonth}, but found {currentMonth}.");
                }
            }

            Context.Set("ServiceMonth", serviceMonth);
            Context.Set("ServiceDateRequestsCount", createClaimsRequest.ServiceDateRequests.Count);
            Context.Set("TreatmentType", createClaimsRequest.ServiceDateRequests.ElementAt(0).TreatmentType);

            Log.Information("[SUCCESS] Invoice data loaded and service month validated: {ServiceMonth}.", serviceMonth);

            return Task.CompletedTask;
        }
    }
}
