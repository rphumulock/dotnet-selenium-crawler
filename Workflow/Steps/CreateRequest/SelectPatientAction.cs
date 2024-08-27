using OpenQA.Selenium;
using Serilog;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class SelectPatientAction : WorkflowStepBase
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Selecting patient...");

            try
            {
                IWebElement patientGrid = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrid")));
                patientGrid.Click();

                Log.Information("[SUCCESS] Patient selected successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while selecting patient: {Message}", ex.Message);
                throw;
            }
        }
    }
}
