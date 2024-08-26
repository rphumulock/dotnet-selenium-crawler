using OpenQA.Selenium;
using HAI_Selenium.Utilities;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class LoadDataAction(WorkflowContext context) : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; } = context;

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Loading JSON data...");

            try
            {
                // Load JSON data for InvoiceRequest and PaymentData
                CreateClaimsRequest createClaimsRequest = FileUtils.LoadJsonFile<CreateClaimsRequest>("Utilities/mockData/InvoiceCreateClaimsRequest.json");
                PaymentData paymentData = FileUtils.LoadJsonFile<PaymentData>("Utilities/mockData/PaymentBreakdown.json");
                Console.WriteLine("[INFO] JSON data loaded successfully.");

                // Set data in the workflow context
                Context.Set("CreateClaimsRequest", createClaimsRequest);
                Context.Set("PaymentData", paymentData);

                Console.WriteLine("[INFO] Invoice and PaymentData set in context.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An error occurred while loading JSON data: {ex.Message}");
                throw;
            }
        }
    }
}
