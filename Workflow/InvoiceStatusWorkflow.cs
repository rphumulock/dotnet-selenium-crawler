using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.Invoice;
using HAI_Selenium.Workflow.AbstractClasses;
using HAI_Selenium.Workflow.Steps.Shared;
using HAI_Selenium.Workflow.Steps.RequestStatus;

namespace HAI_Selenium.Workflow
{
    public class InvoiceStatusWorkflow : InvoiceWorkflowTemplate
    {

        private readonly WorkflowContext Context;

        public InvoiceStatusWorkflow()
        {
            Context = new WorkflowContext();
        }

        protected override void ProcessData(IWebDriver driver)
        {
            try
            {
                WorkflowChain loadDataChain = new WorkflowChain()
                    .AddStep(new LoadStatusInvoiceDataAction(Context));
                loadDataChain.Execute(driver);

                WorkflowChain workflowChain = new WorkflowChain()
                    .AddStep(new NavigateToSiteAction())
                    .AddStep(new LoginAction())
                    .AddStep(new NavigateToClaimsStatusAction());

                Context.Set("ClaimStatuses", new List<ClaimStatusContainer>());

                var indexedBatchedServiceDates = Context.Get<StatusInvoice>("Invoice").ClaimRequests.Select((claimRequest, index) => new { claimRequest, index }).ToList();
                foreach (var indexItem in indexedBatchedServiceDates)
                {
                    workflowChain
                        .AddStep(new FindClaimAction(indexItem.claimRequest))
                        .AddStep(new ProcessClaimHeaderAction(Context))
                        .AddStep(new OpenClaimsLineItemsAction(Context))
                        .AddStep(new ProcessClaimLineItemsAction(Context))
                        .AddStep(new CreateClaimStatusAction(Context));

                }
                workflowChain.Execute(driver);

                List<ClaimStatusContainer> cs = Context.Get<List<ClaimStatusContainer>>("ClaimStatuses");

                foreach (var item in cs)
                {
                    Console.WriteLine($"Things: {item}");
                }

                driver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during workflow process: {ex.Message}");
                throw; // Re-throw to allow outer logic to handle retries
            }
        }
    }
}
