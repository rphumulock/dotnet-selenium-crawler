using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Workflow.Steps.Shared;
using HAI_Selenium.Workflow.Steps.CreateRequest;
using HAI_Selenium.Database.Models;
using HAI_Selenium.Workflow.Steps;
using HAI_Selenium.Services;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Workflow.Workflows
{
    internal class InvoiceRequestWorkflow : InvoiceWorkflowTemplate
    {
        private readonly WorkflowContext _context;
        private readonly IInvoiceRequestService _invoiceRequestService;

        public InvoiceRequestWorkflow(IInvoiceRequestService invoiceRequestService, InvoiceRequest mockRequest)
        {
            _context = new WorkflowContext();
            _invoiceRequestService = invoiceRequestService;
            _context.Set("MockRequest", mockRequest);
        }

 
        protected override async Task InitializeDataAsync(IWebDriver driver)
        {
            var initialDataLoadChain = new WorkflowChain()
                .AddStep(new SetupInvoiceData(_context, _invoiceRequestService))
                .AddStep(new GetPaymentData(_context))
                .AddStep(new ValidateCreateRequestAction(_context))
                .AddStep(new SetServiceDatesFormData(_context));

            await initialDataLoadChain.ExecuteAsync(driver);
        }

        protected override async Task ProcessDataAsync(IWebDriver driver)
        {
            try
            {
                await ExecuteCompileFormDataChain(driver);
                await ProcessBatches(driver);

                InvoiceRequest mockRequest = _context.Get<InvoiceRequest>("MockRequest");
                await _invoiceRequestService.DeleteServiceDateRequestsByInvoiceIdAsync(int.Parse(mockRequest.InvoiceId));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing data.");
                await ErrorHandlerUtils.AnalyzeAndHandleFinalExceptionAsync(ex, _invoiceRequestService);
            }
        }

        private async Task ExecuteCompileFormDataChain(IWebDriver driver)
        {
            var compileFormDataChain = new WorkflowChain()
                .AddStep(new NavigateToSiteAction(_context))
                .AddStep(new LoginAction(_context))
                .AddStep(new NavigateToMembershipSearchAction(_context))
                .AddStep(new FindPatientAction(_context))
                .AddStep(new SelectPatientAction(_context))
                .AddStep(new NavigateToAuthorizationRequestsAction(_context))
                .AddStep(new SelectServiceRequestAction(_context))
                .AddStep(new SetFormHeaderData(_context))
                .AddStep(new SetPaymentData(_context))
                .AddStep(new NavigateToAddClaimsAction(_context));

            await compileFormDataChain.ExecuteAsync(driver);
        }

        protected async Task ProcessBatches(IWebDriver driver)
        {
            var batchServiceDateFormData = _context.Get<List<List<ClaimServiceDateFormData>>>("BatchServiceDateFormData");
            var batchServiceDateRequests = _context.Get<ICollection<ICollection<ServiceDateRequest>>>("BatchServiceDateRequests");

            for (int i = 0; i < batchServiceDateFormData.Count; i++)
            {
                SetBatchContext(i, batchServiceDateFormData, batchServiceDateRequests);
                await ExecuteProcessFormDataChain(driver);
            }
        }

        private void IntroduceError(List<ClaimServiceDateFormData> serviceDateFormData)
        {
            serviceDateFormData[0] = new ClaimServiceDateFormData
            {
                StartDate = "asfdsdf",
                PlaceOfService = "15",
                CPT = "H2016",
                DiagnosisPointer = "GetDiagnosisPointer(mockRequest.DiagnosisCodes.Count)",
                ChargesDollars = "1.",
                ChargesCents = "00",
                Units = "1"
            };
        }

        private void SetBatchContext(int index, List<List<ClaimServiceDateFormData>> batchServiceDateFormData, ICollection<ICollection<ServiceDateRequest>> batchServiceDateRequests)
        {
            var currentBatch = batchServiceDateFormData[index];

            _context.Set("CurrentBatchServiceDateFormData", currentBatch);
            _context.Set("RemainingBatchesServiceDateFormData", batchServiceDateFormData.Skip(index + 1).ToList());

            _context.Set("CurrentBatchServiceDateRequests", batchServiceDateRequests.ElementAt(index));
            _context.Set("RemainingBatchesServiceDateRequests", batchServiceDateRequests.Skip(index + 1).ToList());
        }


        private async Task ExecuteProcessFormDataChain(IWebDriver driver)
        {
            var processFormDataChain = new WorkflowChain()
                .AddStep(new CaptureButtonsAction(_context))
                .AddStep(new AddClaimAction(_context))
                .AddStep(new ProcessClaimFormHeaderAction(_context))
                .AddStep(new ProcessFormServiceDatesAction(_context))
                .AddStep(new ProcessClaimFormFooterAction(_context))
                .AddStep(new CancelClaimAction(_context));

            try
            {
                await processFormDataChain.ExecuteAsync(driver);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing workflow chain.");
                throw;
            }
        }

    }
}
