using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{

    internal class SetServiceDatesFormData(WorkflowContext context) : WorkflowStepBase(context)
    {

        private const int BatchSize = 6;

        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Creating FormData for Processing...");

            InvoiceRequest createClaimsRequest = Context.Get<InvoiceRequest>("InvoiceRequest");
            ICollection<ServiceDateRequest> serviceDateRequests = createClaimsRequest.ServiceDateRequests;
            List<ClaimServiceDateFormData> serviceDateFormDataList = serviceDateRequests.Select(serviceDateRequest => new ClaimServiceDateFormData
            {
                StartDate = serviceDateRequest.ServiceDate,
                PlaceOfService = "15",
                CPT = "H2016",
                DiagnosisPointer = GetDiagnosisPointer(createClaimsRequest.DiagnosisCodes.Count),
                ChargesDollars = "1.",
                ChargesCents = "00",
                Units = "1"
            }).ToList();

            Context.Set("BatchServiceDateRequests", BatchICollection(serviceDateRequests));
            Context.Set("BatchServiceDateFormData", BatchList(serviceDateFormDataList));

            Log.Information("[SUCCESS] FormData for Processing created and stored in context.");
        }

        private static string GetDiagnosisPointer(int diagnosisCodeCount)
        {
            return new string(Enumerable.Range('A', diagnosisCodeCount).Select(x => (char)x).ToArray());
        }

        private static ICollection<ICollection<T>> BatchICollection<T>(ICollection<T> items)
        {
            // Convert the ICollection to a List to use indexing
            var itemsList = items.ToList();
            var batchedItems = new List<ICollection<T>>(); // Use ICollection<T> instead of List<T>

            for (int i = 0; i < itemsList.Count; i += BatchSize)
            {
                // Create a batch with the appropriate size and convert it to ICollection<T>
                var batch = itemsList.GetRange(i, Math.Min(BatchSize, itemsList.Count - i));
                batchedItems.Add(batch);
            }

            return batchedItems;
        }


        private static List<List<T>> BatchList<T>(List<T> items)
        {
            var batchedItems = new List<List<T>>();

            for (int i = 0; i < items.Count; i += BatchSize)
            {
                batchedItems.Add(items.GetRange(i, Math.Min(BatchSize, items.Count - i)));
            }

            return batchedItems;
        }
    }
}
