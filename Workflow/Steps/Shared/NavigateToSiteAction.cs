using HAI_Selenium.Utils;
using OpenQA.Selenium;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    public class NavigateToSiteAction : WorkflowStepBase
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Navigating to Site...");

            try
            {
                string url = EnvironmentUtils.GetEnvironmentVariableOrThrow("URL");
                driver.Navigate().GoToUrl(url);

                Console.WriteLine($"[SUCCESS] Navigated to {url}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to navigate to site: {ex.Message}");
                throw;
            }
        }
    }
}
