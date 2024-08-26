using OpenQA.Selenium;
using HAI_Selenium.Utilities;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Setup
                EnvironmentUtils.LoadEnvVariables();
                EnvironmentUtils.LogCurrentUserInfo();

                // Get the action from environment and create workflow
                string action = EnvironmentUtils.GetEnvironmentVariableOrThrow("ACTION");
                var workflow = WorkflowFactory.GetWorkflow(action);

                // Execute the workflow with retry logic
                IWebDriver driver = WebDriverUtils.SetupDriver();
                WorkflowExecutor.ExecuteWithRetry(workflow, driver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during workflow execution: {ex.Message}");
            }
            finally
            {
               
            }
        }
    }
}
