using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Database.Models;


namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class SetFormHeaderData(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Creating FormData for Processing...");

            InvoiceRequest InvoiceRequest = Context.Get<InvoiceRequest>("InvoiceRequest");
            IncedoServiceRequest LatestServiceRequest = Context.Get<IncedoServiceRequest>("LatestServiceRequest");
            ClaimHeaderFormData formHeaderData = new()
            {
                AuthorizationNumber = LatestServiceRequest.SRAuth,
                PolicyNumber = InvoiceRequest.PolicyNumber,
                DiagnosisCodes = InvoiceRequest.DiagnosisCodes.Select(code => code.Replace(".", "")).ToList(),
            };

            Context.Set("FormHeaderData", formHeaderData);

            Log.Information("[SUCCESS] FormData for Processing created and stored in context.");
        }
    }
}
