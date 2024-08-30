using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;


namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class SetFormHeaderData(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Creating FormData for Processing...");

            InvoiceRequest mockRequest = Context.Get<InvoiceRequest>("MockRequest");
            IncedoServiceRequest LatestServiceRequest = Context.Get<IncedoServiceRequest>("LatestServiceRequest");
            ClaimHeaderFormData formHeaderData = new()
            {
                AuthorizationNumber = LatestServiceRequest.SRAuth,
                PolicyNumber = mockRequest.PolicyNumber,
                DiagnosisCodes = mockRequest.DiagnosisCodes.Select(code => code.Replace(".", "")).ToList(),
            };

            Context.Set("FormHeaderData", formHeaderData);

            Log.Information("[SUCCESS] FormData for Processing created and stored in context.");

            return Task.CompletedTask;
        }
    }
}
