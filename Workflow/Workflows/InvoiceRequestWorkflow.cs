//using OpenQA.Selenium;
//using Serilog;
//using Microsoft.EntityFrameworkCore;
//using HAI_Selenium.Data;
//using HAI_Selenium.InternalClasses.CreateRequest;
//using HAI_Selenium.Workflow.Classes;
//using HAI_Selenium.Workflow.Steps.Shared;
//using HAI_Selenium.Workflow.Steps.CreateRequest;
//using HAI_Selenium.Database.Models;
//using System.Collections.Generic;

//namespace HAI_Selenium.Workflow.Workflows
//{
//    internal class InvoiceRequestWorkflow : InvoiceWorkflowTemplate
//    {
//        private readonly WorkflowContext _context;

//        public InvoiceRequestWorkflow()
//        {
//            _context = new WorkflowContext();
//        }

//        protected override async Task LoadDataAsync(IWebDriver driver)
//        {
//            var mockRequestChain = new WorkflowChain()
//                .AddStep(new LoadDataAction(_context))
//                .AddStep(new ValidateCreateRequestAction(_context));

//            mockRequestChain.Execute(driver);

//            var createClaimsRequest = _context.Get<InvoiceRequest>("InvoiceRequest");

//            using (var dbContext = new ApplicationDbContext())
//            {
//                int invoiceId = createClaimsRequest.Id;
//                var failedInvoiceRequest = await dbContext.InvoiceRequests
//                    .Include(ir => ir.ServiceDateRequests)
//                    .SingleOrDefaultAsync(ir => ir.Id == invoiceId);

//                if (failedInvoiceRequest != null)
//                {
//                    Log.Information("Item with ID {ItemId} exists in the database.", failedInvoiceRequest.Id);
//                    _context.Set("InvoiceRequest", failedInvoiceRequest);
//                }
//                else
//                {
//                    Log.Information("Item with ID {ItemId} does not exist in the database.", createClaimsRequest.Id);
//                }
//            }

//            var createInitialFormDataChain = new WorkflowChain()
//                .AddStep(new SetServiceDatesFormData(_context));

//            createInitialFormDataChain.Execute(driver);
//        }

//        protected override void ProcessData(IWebDriver driver)
//        {
//            var compileFormDataChain = new WorkflowChain()
//                .AddStep(new NavigateToSiteAction(_context))
//                .AddStep(new LoginAction(_context))
//                .AddStep(new NavigateToMembershipSearchAction(_context))
//                .AddStep(new FindPatientAction(_context))
//                .AddStep(new SelectPatientAction(_context))
//                .AddStep(new NavigateToAuthorizationRequestsAction(_context))
//                .AddStep(new SelectServiceRequestAction(_context))
//                .AddStep(new SetFormHeaderData(_context))
//                .AddStep(new SetPaymentData(_context))
//                .AddStep(new NavigateToAddClaimsAction(_context));

//            compileFormDataChain.Execute(driver);

//            int batchSize = _context.Get<int>("BatchSize");
//            List<List<ClaimServiceDateFormData>> batchServiceDateFormData = _context.Get<List<List<ClaimServiceDateFormData>>>("BatchServiceDateFormData");
//            List<ICollection<ServiceDateRequest>> batchServiceDateRequests = _context.Get<List<ICollection<ServiceDateRequest>>>("BatchServiceDateRequests");

//            for (int i = 0; i < batchSize; i++)
//            {
//                var currentBatchFormData = batchServiceDateFormData[i];
//                var currentBatchRequests = batchServiceDateRequests[i];

//                _context.Set("CurrentBatchServiceDateFormData", currentBatchFormData);
//                var remainingBatchesFormData = batchServiceDateFormData.Skip(i + 1).ToList();
//                _context.Set("RemainingBatchesServiceDateFormData", remainingBatchesFormData);

//                _context.Set("CurrentBatchServiceDateRequests", currentBatchRequests);
//                var remainingBatchesRequests = batchServiceDateRequests.Skip(i + 1).ToList();
//                _context.Set("RemainingBatchesServiceDateRequests", remainingBatchesRequests);

//                if (i == 1 && currentBatchFormData.Count > 0)
//                {
//                    currentBatchFormData[0] = new ClaimServiceDateFormData()
//                    {
//                        StartDate = "234234",
//                        PlaceOfService = "15",
//                        CPT = "H2016",
//                        DiagnosisPointer = "234",
//                        ChargesDollars = "1",
//                        ChargesCents = "00",
//                        Units = "1"
//                    };
//                }

//                var processFormDataChain = new WorkflowChain()
//                    .AddStep(new CaptureButtonsAction(_context))
//                    .AddStep(new AddClaimAction(_context))
//                    .AddStep(new ProcessClaimFormHeaderAction(_context))
//                    .AddStep(new ProcessFormServiceDatesAction(_context))
//                    .AddStep(new ProcessClaimFormFooterAction(_context))
//                    .AddStep(new CancelClaimAction(_context));

//                try
//                {
//                    processFormDataChain.Execute(driver);
//                }
//                catch (Exception ex)
//                {
//                    Log.Error(ex, "Error processing workflow chain.");
//                    throw;
//                }
//            }

//        }
//    }
//}
using OpenQA.Selenium;
using Serilog;
using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Data;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Workflow.Steps.Shared;
using HAI_Selenium.Workflow.Steps.CreateRequest;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Workflows
{
    internal class InvoiceRequestWorkflow : InvoiceWorkflowTemplate
    {
        private readonly WorkflowContext _context;

        public InvoiceRequestWorkflow()
        {
            _context = new WorkflowContext();
        }

        protected override async Task LoadDataAsync(IWebDriver driver)
        {
            ExecuteMockRequestChain(driver);
            await LoadInvoiceDataFromDatabaseAsync();
            ExecuteMockRequestChains(driver);
            ExecuteInitialFormDataChain(driver);
        }

        private void ExecuteMockRequestChain(IWebDriver driver)
        {
            var mockRequestChain = new WorkflowChain()
                .AddStep(new LoadDataAction(_context));

            mockRequestChain.Execute(driver);
        }

        private void ExecuteMockRequestChains(IWebDriver driver)
        {
            var mockRequestChain = new WorkflowChain()
                .AddStep(new ValidateCreateRequestAction(_context));

            mockRequestChain.Execute(driver);
        }

        private async Task LoadInvoiceDataFromDatabaseAsync()
        {
            var createClaimsRequest = _context.Get<InvoiceRequest>("InvoiceRequest");

            using (var dbContext = new ApplicationDbContext())
            {
                var invoiceId = createClaimsRequest.InvoiceId;
                var failedInvoiceRequest = await dbContext.InvoiceRequests
                    .Include(ir => ir.ServiceDateRequests)
                    .SingleOrDefaultAsync(ir => ir.InvoiceId == invoiceId);

                if (failedInvoiceRequest != null)
                {
                    Log.Information("Item with ID {ItemId} exists in the database.", failedInvoiceRequest.InvoiceId);
                    _context.Set("InvoiceRequest", failedInvoiceRequest);
                }
                else
                {
                    Log.Warning("Item with ID {ItemId} does not exist in the database.", createClaimsRequest.InvoiceId);
                }
            }
            var createClaimsRequests = _context.Get<InvoiceRequest>("InvoiceRequest");
            _context.Set("ServiceDateRequests", createClaimsRequests.ServiceDateRequests);
            _context.Set("ServiceDatesCount", createClaimsRequest.ServiceDateRequests.Count);
            _context.Set("TreatmentType", createClaimsRequest.ServiceDateRequests.ElementAt(0).TreatmentType);
        }

        private void ExecuteInitialFormDataChain(IWebDriver driver)
        {
            var createInitialFormDataChain = new WorkflowChain()
                .AddStep(new SetServiceDatesFormData(_context));

            createInitialFormDataChain.Execute(driver);
        }

        protected override async Task ProcessDataAsync(IWebDriver driver)
        {
            try
            {
                ExecuteCompileFormDataChain(driver);
                ProcessBatches(driver);

                var createClaimsRequest = _context.Get<InvoiceRequest>("InvoiceRequest");
                await DeleteInvoiceIfExistsAsync(createClaimsRequest.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while processing data.");
                throw;
            }
        }

        private void ExecuteCompileFormDataChain(IWebDriver driver)
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

            compileFormDataChain.Execute(driver);
        }

        // TODO: check catch no working when setting batches for  var batchServiceDateRequests = _context.Get<ICollection<ICollection<ServiceDateRequest>>>("BatchServiceDateRequests"); to list
        // fix errors for driver not closing between runs of serviode dates
        // delete between runs of service dates


        private void ProcessBatches(IWebDriver driver)
        {
            var batchServiceDateFormData = _context.Get<List<List<ClaimServiceDateFormData>>>("BatchServiceDateFormData");
            var batchServiceDateRequests = _context.Get<ICollection<ICollection<ServiceDateRequest>>>("BatchServiceDateRequests");

            for (int i = 0; i < batchServiceDateFormData.Count; i++)
            {
                SetBatchContext(i, batchServiceDateFormData, batchServiceDateRequests);

                if (i == 1 && batchServiceDateFormData[i].Count > 0)
                {
                    SimulateError(batchServiceDateFormData[i]);
                }

                ExecuteProcessFormDataChain(driver);
            }
        }

        private void SetBatchContext(int i, List<List<ClaimServiceDateFormData>> batchServiceDateFormData, ICollection<ICollection<ServiceDateRequest>> batchServiceDateRequests)
        {
            var currentBatchFormData = batchServiceDateFormData[i];
            var currentBatchRequests = batchServiceDateRequests.ElementAt(i);

            _context.Set("CurrentBatchServiceDateFormData", currentBatchFormData);
            _context.Set("RemainingBatchesServiceDateFormData", batchServiceDateFormData.Skip(i + 1).ToList());

            _context.Set("CurrentBatchServiceDateRequests", currentBatchRequests);
            _context.Set("RemainingBatchesServiceDateRequests", batchServiceDateRequests.Skip(i + 1).ToList());
        }

        private void SimulateError(List<ClaimServiceDateFormData> currentBatchFormData)
        {
            currentBatchFormData[0] = null;
        }

        private void ExecuteProcessFormDataChain(IWebDriver driver)
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
                processFormDataChain.Execute(driver);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing workflow chain.");
                throw;
            }
        }

        private async Task DeleteInvoiceIfExistsAsync(int invoiceId)
        {
            Log.Information($"Deleteing Invoice if it exists.", invoiceId);

            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    // Find the InvoiceRequest by Id and include its related ServiceDateRequests
                    var invoiceRequest = await dbContext.InvoiceRequests
                        .Include(ir => ir.ServiceDateRequests)
                        .SingleOrDefaultAsync(ir => ir.Id == invoiceId);

                    // If the InvoiceRequest exists, delete it along with its related ServiceDateRequests
                    if (invoiceRequest != null)
                    {
                        dbContext.InvoiceRequests.Remove(invoiceRequest);
                        await dbContext.SaveChangesAsync();

                        Log.Information("InvoiceRequest with ID {ItemId} and its related ServiceDateRequests have been deleted.", invoiceId);
                    }
                    else
                    {
                        Log.Warning("InvoiceRequest with ID {ItemId} does not exist in the database.", invoiceId);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting InvoiceRequest with ID {ItemId} from the database.", invoiceId);
                throw; // Re-throw exception if you want to handle it further up the call stack
            }
        }
    }
}
