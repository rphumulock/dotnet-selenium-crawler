using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class LoadDataAction : WorkflowStepBase
    {
        protected WorkflowContext Context { get; init; }

        public LoadDataAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Loading JSON data...");

            try
            {
                // Load JSON data for InvoiceRequest and PaymentData
                CreateClaimsRequest createClaimsRequest = FileUtils.LoadJsonFile<CreateClaimsRequest>("Utilities/mockData/InvoiceCreateClaimsRequest.json");
                PaymentData paymentData = FileUtils.LoadJsonFile<PaymentData>("Utilities/mockData/PaymentBreakdown.json");
                Log.Information("JSON data loaded successfully.");

                // Set data in the workflow context
                Context.Set("CreateClaimsRequest", createClaimsRequest);
                Context.Set("PaymentData", paymentData);

                Log.Information("[SUCCESS] Invoice and PaymentData set in context.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while loading JSON data: {Message}", ex.Message);
                throw;
            }
        }
    }
}
