using HAI_Selenium.InternalClasses.Request;
using HAI_Selenium.Workflow.AbstractClasses;
using HAI_Selenium.Workflow.Steps.Shared;
using HAI_Selenium.Workflow.Steps.RequestCreate;
using OpenQA.Selenium;

namespace HAI_Selenium.Workflow
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
                WorkflowChain compileDataChain = new WorkflowChain()
                    .AddStep(new LoadRequestInvoiceDataAction(Context))
                    .AddStep(new ValidateInvoiceRequestAction(Context))
                    .AddStep(new NavigateToSiteAction())
                    .AddStep(new LoginAction())
                    .AddStep(new NavigateToMembershipSearchAction())
                    .AddStep(new FindPatientAction(Context))
                    .AddStep(new SelectPatientAction())
                    .AddStep(new NavigateToAuthorizationRequestsAction())
                    .AddStep(new SelectServiceRequestAction(Context))
                    .AddStep(new CreateFormDataForProcessingAction(Context))
                    .AddStep(new NavigateToAddClaimsAction());
                compileDataChain.Execute(driver);

                WorkflowChain processDataChain = new WorkflowChain();
                List<List<ServiceDateFormData>> batchedServiceDates = Context.Get<List<List<ServiceDateFormData>>>("BatchedServiceDates");
                var indexedBatchedServiceDates = batchedServiceDates.Select((serviceDatesBatch, index) => new { serviceDatesBatch, index }).ToList();
                foreach (var indexItem in indexedBatchedServiceDates)
                {
                    var batchNumber = indexItem.index + 1;

                    processDataChain
                        .AddStep(new CaptureButtonsAction(Context))
                        .AddStep(new AddClaimAction(Context))
                        .AddStep(new ProcessClaimFormHeaderAction(Context))
                        .AddStep(new ProcessFormServiceDatesAction(indexItem.serviceDatesBatch, batchNumber))
                        .AddStep(new ProcessClaimFormFooterAction())
                        .AddStep(new CancelClaimAction(Context));
                }
                processDataChain.Execute(driver);

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
