using HAI_Selenium.Database.Models;
using HAI_Selenium.Workflow.Classes;
using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Services;

namespace HAI_Selenium.Workflow.Steps
{
    public class SetupInvoiceData : WorkflowStepBase
    {
        private readonly IInvoiceRequestService _invoiceRequestService;

        public SetupInvoiceData(WorkflowContext context, IInvoiceRequestService invoiceRequestService)
            : base(context)
        {
            _invoiceRequestService = invoiceRequestService;
        }

        protected override async Task PerformStepAsync(IWebDriver driver)
        {
            InvoiceRequest mockRequest = Context.Get<InvoiceRequest>("MockRequest");

            // Check for the invoice in the database
            var failedInvoiceRequest = await _invoiceRequestService.GetInvoiceRequestByIdAsync(mockRequest.InvoiceId);
            if (failedInvoiceRequest != null)
            {
                Log.Information("Item with ID {ItemId} exists in the database.", failedInvoiceRequest.InvoiceId);
                Context.Set("LoadFromDB", true);
                Context.Set("InvoiceRequest", failedInvoiceRequest);
                Context.Set("ServiceDateRequests", failedInvoiceRequest.ServiceDateRequests);
            }
            else
            {
                Log.Warning("Item with ID {ItemId} does not exist in the database.", mockRequest.InvoiceId);
                Context.Set("LoadFromDB", false);
                Context.Set("ServiceDateRequests", mockRequest.ServiceDateRequests);
            }
        }
    }
}

