using OpenQA.Selenium;

internal class SelectPatientAction : WorkflowStepBase
{
    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine("[ACTION] Selecting patient...");

        try
        {
            IWebElement patientGrid = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrid")));
            patientGrid.Click();

            Console.WriteLine("[INFO] Patient selected successfully.");
        }
        catch (WebDriverTimeoutException ex)
        {
            Console.WriteLine($"[ERROR] Timeout while selecting patient: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] An unexpected error occurred while selecting patient: {ex.Message}");
            throw;
        }
    }
}
