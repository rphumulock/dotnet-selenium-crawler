using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class ProcessClaimFormFooterAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Processing form service dates...");

            IWebElement einNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFedTaxID")));
            WaitUntil(driver, driver => !string.IsNullOrEmpty(einNumberInput.GetAttribute("value")));
            Log.Information("Verified EIN number.");

            IWebElement physPhoneInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPhysPhone")));
            WaitUntil(driver, driver => !string.IsNullOrEmpty(physPhoneInput.GetAttribute("value")));
            Log.Information("Verified physician's phone number.");

            IWebElement physSignedDateInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPhysicianSignedDate")));
            WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(physSignedDateInput, DateTime.Now.ToString("MM/dd/yyyy")));
            Log.Information("Verified physician's signed date.");

            Log.Information("[SUCCESS] Processing form service dates...");
        }
    }
}
