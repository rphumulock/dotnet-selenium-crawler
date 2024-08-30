using OpenQA.Selenium;
using System.Globalization;
using Serilog;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Database.Models;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    public class ValidateCreateRequestAction(WorkflowContext context) : WorkflowStepBase(context)
    {

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Validating service dates month ...");

            InvoiceRequest mockRequest = Context.Get<InvoiceRequest>("MockRequest");
            ICollection<ServiceDateRequest> serviceDateRequests = Context.Get<ICollection<ServiceDateRequest>>("ServiceDateRequests");

            if (serviceDateRequests == null || serviceDateRequests.Count == 0)
            {
                throw new ArgumentNullException("ServiceDateRequests cannot be null or empty.");
            }

            string serviceMonth = null;
            DateTime currentDate = DateTime.Today;
            foreach (var serviceDateRequest in serviceDateRequests)
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

            Log.Information("[SUCCESS] Invoice data loaded and service month validated: {ServiceMonth}.", serviceMonth);

            return Task.CompletedTask;
        }
    }
}
