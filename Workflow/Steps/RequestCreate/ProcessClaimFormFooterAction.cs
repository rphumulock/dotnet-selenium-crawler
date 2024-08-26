using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.RequestCreate
{
    internal class ProcessClaimFormFooterAction : WorkflowStepBase
    {

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Processing form service dates...");

            try
            {
                IWebElement einNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFedTaxID")));
                WaitUntil(driver, driver => !string.IsNullOrEmpty(einNumberInput.GetAttribute("value")));
                Console.WriteLine("[INFO] Verified EIN number.");

                IWebElement physPhoneInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPhysPhone")));
                WaitUntil(driver, driver => !string.IsNullOrEmpty(physPhoneInput.GetAttribute("value")));
                Console.WriteLine("[INFO] Verified physician's phone number.");

                IWebElement physSignedDateInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPhysicianSignedDate")));
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(physSignedDateInput, DateTime.Now.ToString("MM/dd/yyyy")));
                Console.WriteLine("[INFO] Verified physician's signed date.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing service dates: {ex.Message}");
                throw;
            }
        }
    }
}

