using HAI_Selenium.Workflow.Interfaces;
using HAI_Selenium.Workflow.Workflows;
using HAI_Selenium.Services;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Classes
{
    public static class WorkflowFactory
    {
        public static IWorkflowStrategy GetWorkflow(string action, IInvoiceRequestService invoiceRequestService, InvoiceRequest mockRequest)
        {
            return action switch
            {
                "Create" => new InvoiceRequestWorkflow(invoiceRequestService, mockRequest),
                //"Status" => new InvoiceStatusWorkflow(invoiceRequestService),
                _ => throw new InvalidOperationException("Unknown action"),
            };
        }
    }
}
