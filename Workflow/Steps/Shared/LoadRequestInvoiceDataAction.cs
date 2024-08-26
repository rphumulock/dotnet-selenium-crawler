using HAI_Selenium.InternalClasses.Invoice;
using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.Utils;
using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    internal class LoadRequestInvoiceDataAction(WorkflowContext context) : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; } = context;

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Loading JSON data...");

            try
            {
                // Load JSON data for InvoiceRequest and PaymentData
                RequestInvoice invoice = FileUtils.LoadJsonFile<RequestInvoice>("Utils/mockData/InvoiceRequest.json");
                PaymentData paymentData = FileUtils.LoadJsonFile<PaymentData>("Utils/mockData/PaymentBreakdown.json");
                Console.WriteLine("[INFO] JSON data loaded successfully.");

                // Set data in the workflow context
                Context.Set("Invoice", invoice);
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
