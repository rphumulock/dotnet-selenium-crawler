using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class SelectPatientAction : WorkflowStepBase
{
    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine("[ACTION] Selecting patient...");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            IWebElement patientGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrids")));
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
