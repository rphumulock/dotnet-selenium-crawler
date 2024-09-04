using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.InternalClasses.StatusRequest;
using HAI_Selenium.Services;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Interfaces;
using HAI_Selenium.Workflow.Workflows;
using Microsoft.Extensions.DependencyInjection;

internal static class WorkflowFactory
{
    public static IWorkflowStrategy GetWorkflow(string action, IServiceProvider serviceProvider)
    {
        return action switch
        {
            "Create" => new InvoiceRequestWorkflow(
                serviceProvider.GetRequiredService<IInvoiceRequestService>(),  // Resolves IInvoiceRequestService for "Create"
                FileUtils.LoadJsonFile<InvoiceRequest>("Utilities/mockData/InvoiceCreateClaimsRequest.json")
            ),
            "Status" => new InvoiceStatusWorkflow(
                FileUtils.LoadJsonFile<InvoiceStatusRequest>("Utilities/mockData/InvoiceStatusRequest.json")
            ),
            _ => throw new InvalidOperationException("Unknown action"),
        };
    }
}
