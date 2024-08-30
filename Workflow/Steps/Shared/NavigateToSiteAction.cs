using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.Shared
{

    public class NavigateToSiteAction : WorkflowStepBase
    {

        internal NavigateToSiteAction(WorkflowContext context) : base(context) { }

        protected override Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Navigating to Site...");

            string url = EnvironmentUtils.GetEnvironmentVariableOrThrow("URL");
            driver.Navigate().GoToUrl(url);

            Log.Information("[SUCCESS] Navigated to {Url}", url);

            return Task.CompletedTask;
        }
    }

}
