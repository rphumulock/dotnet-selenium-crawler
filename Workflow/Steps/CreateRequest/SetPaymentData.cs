using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class SetPaymentData(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Creating FormData for Processing...");

            InvoiceRequest InvoiceRequest = Context.Get<InvoiceRequest>("InvoiceRequest");
            int serviceDatesCount = Context.Get<int>("ServiceDatesCount");
            string treatmentType = Context.Get<string>("TreatmentType");

            IncedoServiceRequest LatestServiceRequest = Context.Get<IncedoServiceRequest>("LatestServiceRequest");
            List<List<ClaimServiceDateFormData>> BatchServiceDateFormData = Context.Get<List<List<ClaimServiceDateFormData>>>("BatchServiceDateFormData");
            PaymentCalculator paymentData = Context.Get<PaymentCalculator>("PaymentData");
            ICollection<ServiceDateRequest> serviceDateRequests = InvoiceRequest.ServiceDateRequests;

            var latestServiceDate = FindLatestServiceDate(serviceDateRequests);
            var payDate = CalculatePayDate(LatestServiceRequest, latestServiceDate);
            var payment = paymentData.GetAmount(treatmentType, serviceDatesCount);
            var paymentDollars = payment.Split('.')[0] + ".";
            var paymentCents = payment.Split('.')[1];
            BatchServiceDateFormData.Last().Add(new ClaimServiceDateFormData
            {
                StartDate = payDate,
                PlaceOfService = "15",
                CPT = "H2018",
                DiagnosisPointer = GetDiagnosisPointer(InvoiceRequest.DiagnosisCodes.Count),
                ChargesDollars = paymentDollars,
                ChargesCents = paymentCents,
                Units = "1"
            });

            Log.Information("[SUCCESS] FormData for Processing created and stored in context.");

            return Task.CompletedTask;
        }

        private static string GetDiagnosisPointer(int diagnosisCodeCount)
        {
            return new string(Enumerable.Range('A', diagnosisCodeCount).Select(x => (char)x).ToArray());
        }

        private static string CalculatePayDate(IncedoServiceRequest authNumberServiceRequest, ServiceDateRequest serviceDateRequest)
        {
            Log.Information("[ACTION] Calculating pay date...");

            var authNumberServiceRequestParts = authNumberServiceRequest.StartDate.Split('/');
            var serviceDateRequestParts = serviceDateRequest.ServiceDate.Split('/');

            if (!int.TryParse(authNumberServiceRequestParts[1], out int authNumberServiceRequestDay) ||
                !int.TryParse(serviceDateRequestParts[1], out int serviceDateRequestDay))
            {
                Log.Error("Invalid date format in service date or service request start date.");
                throw new ArgumentException("Invalid date format in service date or service request start date.");
            }

            string payDate;

            if (authNumberServiceRequestDay >= serviceDateRequestDay)
            {
                payDate = string.Join("/", authNumberServiceRequestParts);
            }
            else
            {
                string lastDayOfMonth = GetLastDayOfMonth(serviceDateRequestParts[0], serviceDateRequestParts[2]);
                int payDay = serviceDateRequestParts[1] == lastDayOfMonth ? serviceDateRequestDay : serviceDateRequestDay + 1;
                payDate = $"{serviceDateRequestParts[0]}/{payDay}/{serviceDateRequestParts[2]}";
            }

            return payDate;
        }

        private static string GetLastDayOfMonth(string monthString, string yearString)
        {
            if (!int.TryParse(monthString, out int month) || month < 1 || month > 12)
            {
                Log.Error("Invalid month format.");
                throw new ArgumentException("Invalid month format.");
            }

            if (!int.TryParse(yearString, out int year))
            {
                Log.Error("Invalid year format.");
                throw new ArgumentException("Invalid year format.");
            }

            return DateTime.DaysInMonth(year, month).ToString();
        }

        private static ServiceDateRequest FindLatestServiceDate(ICollection<ServiceDateRequest> serviceDateRequests)
        {
            return serviceDateRequests.OrderByDescending(sd => DateTime.ParseExact(sd.ServiceDate, "MM/dd/yyyy", null)).First();
        }
    }
}
