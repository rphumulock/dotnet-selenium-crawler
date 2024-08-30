using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class GetPaymentData(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Loading JSON data...");

            // Load JSON data for InvoiceRequest and PaymentData
            PaymentCalculator paymentData = FileUtils.LoadJsonFile<PaymentCalculator>("Utilities/mockData/PaymentBreakdown.json");
            Log.Information("JSON data loaded successfully.");

            // Set data in the workflow context
            Context.Set("PaymentData", paymentData);

            Log.Information("[SUCCESS] PaymentData set in context.");

            return Task.CompletedTask;
        }
    }
}
