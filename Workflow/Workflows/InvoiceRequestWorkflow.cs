using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.AbstractClasses;
using HAI_Selenium.Workflow.Steps.Shared;
using HAI_Selenium.Workflow.Steps.CreateRequest;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Workflow.Workflows
{
    internal class InvoiceRequestWorkflow : InvoiceWorkflowTemplate
    {
        private readonly WorkflowContext Context;

        public InvoiceRequestWorkflow()
        {
            Context = new WorkflowContext();
        }

        protected override void ProcessData(IWebDriver driver)
        {
            try
            {
                WorkflowChain loadDataChain = new WorkflowChain()
                    .AddStep(new LoadDataAction(Context))
                    .AddStep(new ValidateCreateRequestAction(Context));
                loadDataChain.Execute(driver);

                WorkflowChain compileFormDataChain = new WorkflowChain()
                   .AddStep(new NavigateToSiteAction())
                    .AddStep(new LoginAction())
                    .AddStep(new NavigateToMembershipSearchAction())
                    .AddStep(new FindPatientAction(Context))
                    .AddStep(new SelectPatientAction())
                    .AddStep(new NavigateToAuthorizationRequestsAction())
                    .AddStep(new SelectServiceRequestAction(Context))
                    .AddStep(new CreateFormDataForProcessingAction(Context))
                    .AddStep(new NavigateToAddClaimsAction());
                compileFormDataChain.Execute(driver);

                List<List<ClaimServiceDateFormData>> batchedServiceDates = Context.Get<List<List<ClaimServiceDateFormData>>>("BatchedServiceDates");
                var indexedBatchedServiceDates = batchedServiceDates.Select((serviceDatesBatch, index) => new { serviceDatesBatch, index }).ToList();
                WorkflowChain processFormDataChain = new WorkflowChain();
                foreach (var indexItem in indexedBatchedServiceDates)
                {
                    var batchNumber = indexItem.index + 1;
                    processFormDataChain
                        .AddStep(new CaptureButtonsAction(Context))
                        .AddStep(new AddClaimAction(Context))
                        .AddStep(new ProcessClaimFormHeaderAction(Context))
                        .AddStep(new ProcessFormServiceDatesAction(indexItem.serviceDatesBatch, batchNumber))
                        .AddStep(new ProcessClaimFormFooterAction())
                        .AddStep(new CancelClaimAction(Context));
                }
                processFormDataChain.Execute(driver);

                driver.Quit();
            }
            catch (RecoverableError ex)
            {
                Console.WriteLine($"[ERROR] Non-recoverable error occurred: {ex.Message}");
                throw; // Re-throw the exception to indicate final failure
            }
            catch (NonRecoverableError ex)
            {
                Console.WriteLine($"[ERROR] Non-recoverable error occurred: {ex.Message}");
                throw; // Re-throw the exception to indicate final failure
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during workflow process: {ex.Message}");
                throw;
            }
        }
    }
}
