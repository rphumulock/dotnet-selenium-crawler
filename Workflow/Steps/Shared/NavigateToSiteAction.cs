using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    public class NavigateToSiteAction : WorkflowStepBase
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Navigating to Site...");

            try
            {
                string url = EnvironmentUtils.GetEnvironmentVariableOrThrow("URL");
                driver.Navigate().GoToUrl(url);

                Log.Information("[SUCCESS] Navigated to {Url}", url);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to navigate to site: {Message}", ex.Message);
                throw;
            }
        }
    }
}
