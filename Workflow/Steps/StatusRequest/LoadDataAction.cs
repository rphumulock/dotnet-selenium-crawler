using OpenQA.Selenium;
using HAI_Selenium.Utilities;
using HAI_Selenium.InternalClasses.StatusRequest;

namespace HAI_Selenium.Workflow.Steps.StatusRequest
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
                InvoiceStatusRequest invoiceStatusRequest = FileUtils.LoadJsonFile<InvoiceStatusRequest>("Utilities/mockData/InvoiceStatusRequest.json");

                Console.WriteLine("[INFO] JSON data loaded successfully.");

                // Set data in the workflow context
                Context.Set("InvoiceStatusRequest", invoiceStatusRequest);

                Console.WriteLine("[INFO] Invoice set in context.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An error occurred while loading JSON data: {ex.Message}");
                throw;
            }
        }
    }
}
