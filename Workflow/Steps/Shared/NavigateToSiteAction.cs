using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Classes;


namespace HAI_Selenium.Workflow.Steps.Shared
{
    public class NavigateToSiteAction : WorkflowStepBase
    {
        internal NavigateToSiteAction(WorkflowContext context) : base(context)
        {
            Context = context;
        }

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
                throw;
            }
        }
    }
}
