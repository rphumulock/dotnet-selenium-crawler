using HAI_Selenium.InternalActions;
using HAI_Selenium.InternalClasses;
using HAI_Selenium.Utils;
using HAISelenium.InternalClasses;
using OpenQA.Selenium;

namespace HAI_Selenium
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxRetries = 3;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                IWebDriver driver = null;
                try
                {
                    // Setup
                    Utilities.LoadEnvVariables();
                    Utilities.LogCurrentUserInfo();
                    driver = Utilities.SetupDriver();

                    ProcessRequest(driver);

                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"[ERROR] An error occurred: {ex.Message}. Attempting retry {retryCount} of {maxRetries}...");

                    driver?.Close();
                    driver?.Quit();

                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("[FATAL] Max retries reached. Exiting program.");
                        throw;
                    }
                }
                //finally
                //{
                //    Console.WriteLine("[INFO] Closing browser...");
                //    driver?.Close();
                //    driver?.Quit();
                //    Console.WriteLine("[INFO] Browser closed.");
                //}
            }
        }

        private static void ProcessRequest(IWebDriver driver)
        {
            // Navigation
            LoginActions.LoginToSite(driver);

            string action = Utilities.GetEnvironmentVariableOrThrow("ACTION");
            if (action == "Create")
            {
                CreateClaimsRequestWorkflow(driver);
            }
            else if (action == "Status")
            {
                CheckClaimsStatusWorkflow(driver);
            }
        }

        private static void CreateClaimsRequestWorkflow(IWebDriver driver)
        {
            // Load Json Data
            Invoice invoice = Utilities.LoadJsonFile<Invoice>("Utils/CreateClaimsRequest.json");
            PaymentData paymentData = Utilities.LoadJsonFile<PaymentData>("Utils/PaymentBreakdown.json");

            PatientActions.SelectPatient(driver, invoice);

            // Prepare Data
            string serviceDatesMonth = Utilities.ValidateServiceDateMonth(invoice);
            ServiceRequest authNumberServiceRequest = ServiceRequestActions.GetServiceRequestWithAuthNumber(driver, serviceDatesMonth);

            FormDataForProcessing formDataForProcessing = Utilities.CreateFormDataForProcessing(invoice, paymentData, authNumberServiceRequest);

            // Process Data
            ClaimsActions.ProcessData(driver, formDataForProcessing);
        }

        private static void CheckClaimsStatusWorkflow(IWebDriver driver)
        {
            // Load Json Data
            StatusInvoice invoice = Utilities.LoadJsonFile<StatusInvoice>("Utils/ClaimStatusRequest.json");

            ClaimsStatusActions.CheckClaimsStatus(driver, invoice);

        }

    }
}
