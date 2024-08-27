using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
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
            Log.Information("[ACTION] Creating FormData for Processing...");

            try
            {
                CreateClaimsRequest createClaimsRequest = Context.Get<CreateClaimsRequest>("CreateClaimsRequest");
                PaymentData paymentData = Context.Get<PaymentData>("PaymentData");
                IncedoServiceRequest authNumberServiceRequest = Context.Get<IncedoServiceRequest>("ServiceRequest");

                List<ServiceDateRequests> serviceDateRequests = createClaimsRequest.ServiceDateRequests;
                var serviceDateFormDataList = serviceDateRequests.Select(serviceDateRequest => new ClaimServiceDateFormData
                {
                    StartDate = serviceDateRequest.ServiceDate,
                    PlaceOfService = "15",
                    CPT = "H2016",
                    DiagnosisPointer = GetDiagnosisPointer(createClaimsRequest.DiagnosisCodes.Count()),
                    ChargesDollars = "1.",
                    ChargesCents = "00",
                    Units = "1"
                }).ToList();

                var latestServiceDate = FindLatestServiceDate(serviceDateRequests);
                var payDate = CalculatePayDate(authNumberServiceRequest, latestServiceDate);
                var payment = paymentData.GetAmount(serviceDateRequests[0].TreatmentType, serviceDateFormDataList.Count);
                var paymentDollars = payment.Split('.')[0] + ".";
                var paymentCents = payment.Split('.')[1];
                serviceDateFormDataList.Add(new ClaimServiceDateFormData
                {
                    StartDate = payDate,
                    PlaceOfService = "15",
                    CPT = "H2018",
                    DiagnosisPointer = GetDiagnosisPointer(createClaimsRequest.DiagnosisCodes.Count()),
                    ChargesDollars = paymentDollars,
                    ChargesCents = paymentCents,
                    Units = "1"
                });

                ClaimHeaderFormData formHeaderData = new ClaimHeaderFormData
                {
                    AuthorizationNumber = authNumberServiceRequest.SRAuth,
                    PolicyNumber = createClaimsRequest.PolicyNumber,
                    DiagnosisCodes = createClaimsRequest.DiagnosisCodes.Select(code => code.Replace(".", "")).ToList(),
                };
                List<List<ClaimServiceDateFormData>> batchedServiceDateFormData = BatchServiceDateFormData(serviceDateFormDataList);

                Context.Set("FormHeaderData", formHeaderData);
                Context.Set("BatchedServiceDates", batchedServiceDateFormData);

                Log.Information("[SUCCESS] FormData for Processing created and stored in context.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while creating FormData for Processing: {Message}", ex.Message);
                throw;
            }
        }

        private static string GetDiagnosisPointer(int diagnosisCodeCount)
        {
            return new string(Enumerable.Range('A', diagnosisCodeCount).Select(x => (char)x).ToArray());
        }

        private static List<List<ClaimServiceDateFormData>> BatchServiceDateFormData(List<ClaimServiceDateFormData> serviceDateFormDataList)
        {
            const int batchSize = 6;
            var batchedServiceDateFormData = new List<List<ClaimServiceDateFormData>>();

            for (int i = 0; i < serviceDateFormDataList.Count; i += batchSize)
            {
                batchedServiceDateFormData.Add(serviceDateFormDataList.GetRange(i, Math.Min(batchSize, serviceDateFormDataList.Count - i)));
            }

            return batchedServiceDateFormData;
        }

        private static string CalculatePayDate(IncedoServiceRequest authNumberServiceRequest, ServiceDateRequests serviceDateRequest)
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

        private static ServiceDateRequests FindLatestServiceDate(List<ServiceDateRequests> serviceDateRequests)
        {
            return serviceDateRequests.OrderByDescending(sd => DateTime.ParseExact(sd.ServiceDate, "MM/dd/yyyy", null)).First();
        }
    }
}
