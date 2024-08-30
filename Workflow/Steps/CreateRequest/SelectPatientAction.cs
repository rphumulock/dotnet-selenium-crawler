using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class SelectPatientAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Selecting patient...");


            IWebElement patientGrid = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrid")));
            patientGrid.Click();

            Log.Information("[SUCCESS] Patient selected successfully.");

        }
    }
}
