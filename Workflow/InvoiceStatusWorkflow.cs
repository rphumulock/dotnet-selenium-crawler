using HAI_Selenium.InternalActions.Claim;
using HAI_Selenium.InternalActions;
using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.Workflow.AbstractClasses;
using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.Invoice;

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
                WorkflowChain workflowChain = new WorkflowChain()
                    .AddStep(new LoadStatusInvoiceDataAction(Context))
                    .AddStep(new NavigateToSiteAction())
                    .AddStep(new LoginAction())
                    .AddStep(new NavigateToClaimsStatusAction());

                StatusInvoice invoice = Context.Get<StatusInvoice>("Invoice");
                var indexedBatchedServiceDates = invoice.ClaimRequests.Select((claimRequest, index) => new { claimRequest, index }).ToList();
                foreach (var indexItem in indexedBatchedServiceDates)
                {
                    var batchNumber = indexItem.index + 1;

                    workflowChain
                        .AddStep(new FindClaimAction(indexItem.claimRequest));

                }
                workflowChain.Execute(driver);


                //            var claimsList = invoice.Claims.Select((claim, index) => new { claim, index }).ToList();
                //            List<ClaimDetails> allClaimsDetails = new List<ClaimDetails>();

                //            foreach (var indexItem in claimsList)
                //            {
                //                Console.WriteLine($"[ACTION] Processing claim for batch #{indexItem.claim.ClaimID}...");
                //                Utilities.Retry(() => FindClaim(driver, indexItem.claim), 3, $"[WARNING] Failed to find claim #{indexItem.claim.ClaimID}. Retrying...");

                //                ClaimDetails claimDetails = Utilities.Retry(() => ProcessClaimStatus(driver, indexItem.claim), 3, $"[WARNING] Failed to process status for claim #{indexItem.claim.ClaimID}. Retrying...");

                //                allClaimsDetails.Add(claimDetails);

                //                //Console.WriteLine($"[INFO] Processed claim #{indexItem.claim.ClaimID}. Claim Details: {claimDetails}");
                //            }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during workflow process: {ex.Message}");
                throw; // Re-throw to allow outer logic to handle retries
            }
        }
    }
}
