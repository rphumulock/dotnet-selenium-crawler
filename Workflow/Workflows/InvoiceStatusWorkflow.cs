//using OpenQA.Selenium;
//using HAI_Selenium.InternalClasses.StatusRequest;
//using HAI_Selenium.Workflow.AbstractClasses;
//using HAI_Selenium.Workflow.Steps.Shared;
//using HAI_Selenium.Workflow.Steps.StatusRequest;

//namespace HAI_Selenium.Workflow.Workflows
//{
//    public class InvoiceStatusWorkflow : InvoiceWorkflowTemplate
//    {

//        private readonly WorkflowContext Context;

//        public InvoiceStatusWorkflow()
//        {
//            Context = new WorkflowContext();
//            Context.Set("ClaimStatuses", new List<ClaimsStatusWithLineItems>());
//        }

//        protected override void ProcessData(IWebDriver driver)
//        {
//            try
//            {
//                WorkflowChain loadDataChain = new WorkflowChain()
//                    .AddStep(new LoadDataAction(Context));
//                loadDataChain.Execute(driver);

//                WorkflowChain workflowChain = new WorkflowChain()
//                    .AddStep(new NavigateToSiteAction())
//                    .AddStep(new LoginAction())
//                    .AddStep(new NavigateToClaimsStatusAction());

//                var indexedBatchedServiceDates = Context.Get<InvoiceStatusRequest>("InvoiceStatusRequest").ClaimStatusRequests.Select((ClaimStatusRequest, index) => new { ClaimStatusRequest, index }).ToList();
//                foreach (var indexItem in indexedBatchedServiceDates)
//                {
//                    workflowChain
//                        .AddStep(new FindClaimAction(indexItem.ClaimStatusRequest))
//                        .AddStep(new ProcessClaimHeaderAction(Context))
//                        .AddStep(new OpenClaimsLineItemsAction(Context))
//                        .AddStep(new ProcessClaimLineItemsAction(Context))
//                        .AddStep(new CreateClaimStatusAction(Context));
//                }
//                workflowChain.Execute(driver);

//                List<ClaimsStatusWithLineItems> cs = Context.Get<List<ClaimsStatusWithLineItems>>("ClaimStatuses");

//                foreach (var item in cs)
//                {
//                    Console.WriteLine($"Claim Status: {item}");
//                }

//                driver.Quit();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error during workflow process: {ex.Message}");
//                throw; // Re-throw to allow outer logic to handle retries
//            }
//        }
//    }
//}
