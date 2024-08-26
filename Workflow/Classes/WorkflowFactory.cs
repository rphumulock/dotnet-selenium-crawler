using HAI_Selenium.Workflow.Interfaces;
using HAI_Selenium.Workflow.Workflows;

namespace HAI_Selenium.Workflow.Classes
{
    public static class WorkflowFactory
    {
        public static IWorkflowStrategy GetWorkflow(string action)
        {
            return action switch
            {
                "Create" => new InvoiceRequestWorkflow(),
                "Status" => new InvoiceStatusWorkflow(),
                _ => throw new InvalidOperationException("Unknown action"),
            };
        }
    }
}
