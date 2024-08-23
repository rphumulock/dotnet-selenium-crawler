using HAI_Selenium.Factories;
using HAI_Selenium.Utils;
using OpenQA.Selenium;

namespace HAI_Selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = null;
            try
            {
                // Setup
                Utilities.LoadEnvVariables();
                Utilities.LogCurrentUserInfo();
                driver = Utilities.SetupDriver();

                ProcessRequest(driver);
            }
            catch (Exception ex)
            {
                driver?.Close();
                driver?.Quit();
            }
        }

        private static void ProcessRequest(IWebDriver driver)
        {
            string action = Utilities.GetEnvironmentVariableOrThrow("ACTION");
            var workflow = WorkflowFactory.GetWorkflow(action);
            workflow.Execute(driver);
        }
    }
}
