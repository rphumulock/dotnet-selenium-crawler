using OpenQA.Selenium;
using System.Globalization;
using HAI_Selenium.InternalClasses.Invoice;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    public class ValidateInvoiceRequestAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        public ValidateInvoiceRequestAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Validating service dates month ...");

            var invoice = Context.Get<RequestInvoice>("Invoice");

            try
            {
                if (invoice.ServiceDateRequests == null || invoice.ServiceDateRequests.Count == 0)
                {
                    throw new ArgumentNullException("[ERROR] ServiceDateRequests cannot be null or empty.");
                }

                string serviceMonth = null;
                DateTime currentDate = DateTime.Today;
                foreach (var serviceDateRequest in invoice.ServiceDateRequests)
                {
                    if (!DateTime.TryParseExact(serviceDateRequest.ServiceDate, new[] { "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy" },
                                                null, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        throw new InvalidOperationException($"[ERROR] Invalid date format: {serviceDateRequest.ServiceDate}");
                    }

                    string currentMonth = parsedDate.Month.ToString("D2");

                    if (parsedDate.Date == currentDate)
                    {
                        throw new InvalidOperationException($"[ERROR] Service date {serviceDateRequest.ServiceDate} cannot be today's date.");
                    }

                    if (serviceMonth == null)
                    {
                        serviceMonth = currentMonth;
                    }
                    else if (currentMonth != serviceMonth)
                    {
                        throw new InvalidOperationException($"[ERROR] Mismatch found: expected month {serviceMonth}, but found {currentMonth}.");
                    }
                }

                Context.Set("ServiceMonth", serviceMonth);

                Console.WriteLine($"[SUCCESS] Invoice data loaded and service month validated: {serviceMonth}.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while finding patient: {ex.Message}");
                throw;
            }
        }
    }
}
