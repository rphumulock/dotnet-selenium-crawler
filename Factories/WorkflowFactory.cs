using HAI_Selenium.Interfaces;
using HAI_Selenium.Workflows;

namespace HAI_Selenium.Factories
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
