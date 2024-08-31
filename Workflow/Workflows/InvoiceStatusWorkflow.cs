using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.StatusRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Workflow.Steps.Shared;
using HAI_Selenium.Services;
using HAI_Selenium.Workflow.Steps.StatusRequest;

namespace HAI_Selenium.Workflow.Workflows
{
    internal class InvoiceStatusWorkflow : InvoiceWorkflowTemplate
    {
        private readonly WorkflowContext _context;
        private readonly INRulesService _nRulesService;

        public InvoiceStatusWorkflow(INRulesService nRulesService, InvoiceStatusRequest mockRequest)
        {
            _context = new WorkflowContext();
            _nRulesService = nRulesService;
            _context.Set("MockRequest", mockRequest);
        }

        protected override void InitializeData(IWebDriver driver)
        {
            _context.Set("ClaimStatuses", new List<ClaimsStatusWithLineItems>());
        }

        protected override async Task ProcessDataAsync(IWebDriver driver)
        {
            try
            {
                await ExecuteWorkflowChain(driver);

                List<ClaimsStatusWithLineItems> claimStatuses = _context.Get<List<ClaimsStatusWithLineItems>>("ClaimStatuses");
                foreach (var item in claimStatuses)
                {
                    Console.WriteLine($"Claim Status: {item}");
                }

                EvaluateRules(claimStatuses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during workflow process.");
                throw;
            }
            finally
            {
                driver.Quit();
            }
        }

        private async Task ExecuteWorkflowChain(IWebDriver driver)
        {
            var workflowChain = new WorkflowChain()
                .AddStep(new NavigateToSiteAction(_context))
                .AddStep(new LoginAction(_context))
                .AddStep(new NavigateToClaimsStatusAction(_context));

            InvoiceStatusRequest mockRequest = _context.Get<InvoiceStatusRequest>("MockRequest");
            var indexedBatchedServiceDates = mockRequest.ClaimStatusRequests
                .Select((claimStatusRequest, index) => new { claimStatusRequest, index })
                .ToList();

            foreach (var indexItem in indexedBatchedServiceDates)
            {
                workflowChain
                    .AddStep(new FindClaimAction(_context, indexItem.claimStatusRequest))
                    .AddStep(new ProcessClaimHeaderAction(_context))
                    .AddStep(new OpenClaimsLineItemsAction(_context))
                    .AddStep(new ProcessClaimLineItemsAction(_context))
                    .AddStep(new CreateClaimStatusAction(_context));
            }

            await workflowChain.ExecuteAsync(driver);
        }

        private void EvaluateRules(List<ClaimsStatusWithLineItems> claimStatuses)
        {
            var session = _nRulesService.CreateSession();

            // Insert facts (your claimStatuses or any other relevant objects)
            foreach (var claimStatus in claimStatuses)
            {
                session.Insert(claimStatus);
            }

            // Fire rules
            session.Fire();
        }
    }
}
