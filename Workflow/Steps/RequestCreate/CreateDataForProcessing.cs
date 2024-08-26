using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.InternalClasses.Invoice;

namespace HAI_Selenium.Workflow.Steps.RequestCreate
{
    internal class CreateFormDataForProcessingAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        public CreateFormDataForProcessingAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Creating FormData for Processing...");

            try
            {
                var invoice = Context.Get<RequestInvoice>("Invoice");
                var paymentData = Context.Get<PaymentData>("PaymentData");
                var authNumberServiceRequest = Context.Get<ServiceRequest>("ServiceRequest");

                var serviceDateRequests = invoice.ServiceDateRequests;
                var serviceDateFormDataList = serviceDateRequests.Select(serviceDateRequest => new ServiceDateFormData
                {
                    StartDate = serviceDateRequest.ServiceDate,
                    PlaceOfService = "15",
                    CPT = "H2016",
                    DiagnosisPointer = GetDiagnosisPointer(invoice.DiagnosisCodes.Count()),
                    ChargesDollars = "1.",
                    ChargesCents = "00",
                    Units = "1"
                }).ToList();

                var latestServiceDate = FindLatestServiceDate(serviceDateRequests);
                var payDate = CalculatePayDate(authNumberServiceRequest, latestServiceDate);
                var payment = paymentData.GetAmount(serviceDateRequests[0].TreatmentType, serviceDateFormDataList.Count);
                var paymentDollars = payment.Split('.')[0] + ".";
                var paymentCents = payment.Split('.')[1];
                serviceDateFormDataList.Add(new ServiceDateFormData
                {
                    StartDate = payDate,
                    PlaceOfService = "15",
                    CPT = "H2018",
                    DiagnosisPointer = GetDiagnosisPointer(invoice.DiagnosisCodes.Count()),
                    ChargesDollars = paymentDollars,
                    ChargesCents = paymentCents,
                    Units = "1"
                });


                RequestInvoiceFormHeaderData formHeaderData = new RequestInvoiceFormHeaderData
                {
                    authNumber = authNumberServiceRequest.SRAuth,
                    patientPolicyNumber = invoice.PolicyNumber,
                    patientDiagnosisCodes = invoice.DiagnosisCodes.Select(code => code.Replace(".", "")).ToList(),

                };
                List<List<ServiceDateFormData>> batchedServiceDateFormData = BatchServiceDateFormData(serviceDateFormDataList);

                Context.Set("FormHeaderData", formHeaderData);
                Context.Set("BatchedServiceDates", batchedServiceDateFormData);

                Console.WriteLine("[SUCCESS] FormData for Processing created and stored in context.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while creating FormData for Processing: {ex.Message}");
                throw;
            }
        }

        private static string GetDiagnosisPointer(int diagnosisCodeCount)
        {
            return new string(Enumerable.Range('A', diagnosisCodeCount).Select(x => (char)x).ToArray());
        }

        private static List<List<ServiceDateFormData>> BatchServiceDateFormData(List<ServiceDateFormData> serviceDateFormDataList)
        {
            const int batchSize = 6;
            var batchedServiceDateFormData = new List<List<ServiceDateFormData>>();

            for (int i = 0; i < serviceDateFormDataList.Count; i += batchSize)
            {
                batchedServiceDateFormData.Add(serviceDateFormDataList.GetRange(i, Math.Min(batchSize, serviceDateFormDataList.Count - i)));
            }

            return batchedServiceDateFormData;
        }

        private static string CalculatePayDate(ServiceRequest authNumberServiceRequest, ServiceDateRequest serviceDateRequest)
        {
            Console.WriteLine("[ACTION] Calculating pay date...");

            var authNumberServiceRequestParts = authNumberServiceRequest.StartDate.Split('/');
            var serviceDateRequestParts = serviceDateRequest.ServiceDate.Split('/');

            if (!int.TryParse(authNumberServiceRequestParts[1], out int authNumberServiceRequestDay) ||
                !int.TryParse(serviceDateRequestParts[1], out int serviceDateRequestDay))
            {
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
                throw new ArgumentException("Invalid month format.");
            }

            if (!int.TryParse(yearString, out int year))
            {
                throw new ArgumentException("Invalid year format.");
            }

            return DateTime.DaysInMonth(year, month).ToString();
        }

        private static ServiceDateRequest FindLatestServiceDate(List<ServiceDateRequest> serviceDateRequests)
        {
            return serviceDateRequests.OrderByDescending(sd => DateTime.ParseExact(sd.ServiceDate, "MM/dd/yyyy", null)).First();
        }
    }
}