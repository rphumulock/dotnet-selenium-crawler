//using OpenQA.Selenium;
//using HAI_Selenium.Utilities;
//using HAI_Selenium.InternalClasses.StatusRequest;
//using Serilog;

//namespace HAI_Selenium.Workflow.Steps.StatusRequest
//{
//    internal class LoadDataAction : WorkflowStepBase
//    {
//        protected WorkflowContext Context { get; init; }

//        internal LoadDataAction(WorkflowContext context)
//        {
//            Context = context;
//        }

//        protected override void PerformStep(IWebDriver driver)
//        {
//            Log.Information("[ACTION] Loading JSON data...");

//            try
//            {
//                // Load JSON data for InvoiceRequest and PaymentData
//                InvoiceStatusRequest invoiceStatusRequest = FileUtils.LoadJsonFile<InvoiceStatusRequest>("Utilities/mockData/InvoiceStatusRequest.json");

//                Log.Information("JSON data loaded successfully.");

//                // Set data in the workflow context
//                Context.Set("InvoiceStatusRequest", invoiceStatusRequest);

//                Log.Information("[SUCCESS] Invoice set in context.");
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex, "An error occurred while loading JSON data: {Message}", ex.Message);
//                throw;
//            }
//        }
//    }
//}
