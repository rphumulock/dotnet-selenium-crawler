using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class ProcessClaimFormFooterAction : WorkflowStepBase
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Processing form service dates...");

            try
            {
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
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while processing service dates: {Message}", ex.Message);
                throw;
            }
        }
    }
}
