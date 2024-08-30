using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Exceptions
{
    internal class HAIException : Exception
    {
        public WorkflowContext Context { get; }
        public HAIException(string message, WorkflowContext context, Exception innerException)
            : base(message, innerException)
        {
            Context = context;
        }
        public HAIException(string message, WorkflowContext context)
            : base(message)
        {
            Context = context;
        }
    }
}
