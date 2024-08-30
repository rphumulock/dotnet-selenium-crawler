using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class MockRequest(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Loading JSON data...");

            // Load JSON data for InvoiceRequest and PaymentData
            InvoiceRequest createClaimsRequest = FileUtils.LoadJsonFile<InvoiceRequest>("Utilities/mockData/InvoiceCreateClaimsRequest.json");
            PaymentCalculator paymentData = FileUtils.LoadJsonFile<PaymentCalculator>("Utilities/mockData/PaymentBreakdown.json");
            Log.Information("JSON data loaded successfully.");

            // Set data in the workflow context
            Context.Set("InvoiceRequest", createClaimsRequest);
            Context.Set("PaymentData", paymentData);

            Log.Information("[SUCCESS] Invoice and PaymentData set in context.");

            return Task.CompletedTask;
        }
    }
}
