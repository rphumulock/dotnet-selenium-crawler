using HAI_Selenium.InternalClasses;
using HAISelenium.InternalActions;
using HAISelenium.InternalClasses;
using HAISelenium.Utils;
using OpenQA.Selenium;

namespace HAISelenium
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
                    Invoice invoice = Utilities.LoadJsonFile<Invoice>("Utils/request.json");

                    // Navigation
                    LoginToSite(driver);
                    SelectPatient(driver, invoice);

                    // Prepare Data
                    string serviceDatesMonth = ValidateServiceDateMonth(invoice);
                    ServiceRequest authNumberServiceRequest = GetServiceRequestWithAuthNumber(driver, serviceDatesMonth);

                    FormDataForProcessing formDataForProcessing = Utilities.CreateFormDataForProcessing(invoice, authNumberServiceRequest);

                    // Process Data
                    ProcessData(driver, formDataForProcessing);

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
        internal static string ValidateServiceDateMonth(Invoice invoice)
        {
            Console.WriteLine("[ACTION] Validating service dates month ...");

            if (invoice.ServiceDateRequests == null || invoice.ServiceDateRequests.Count == 0)
            {
                throw new ArgumentNullException("[ERROR] ServiceDateRequests cannot be null or empty.");
            }

            string serviceDateMonth = null;

            foreach (var ServiceDateRequest in invoice.ServiceDateRequests)
            {
                string[] dateParts = ServiceDateRequest.ServiceDate.Split('/');
                string currentMonth = dateParts[0];

                if (serviceDateMonth == null)
                {
                    serviceDateMonth = currentMonth;
                }
                else if (currentMonth != serviceDateMonth)
                {
                    throw new InvalidOperationException($"[ERROR] Mismatch found: expected month {serviceDateMonth}, but found {currentMonth}.");
                }
            }

            Console.WriteLine($"[SUCCESS] Invoice data loaded and service month validated: {serviceDateMonth}.");

            return serviceDateMonth;
        }


        internal static void LoginToSite(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Logging into the site...");

            Utilities.Retry(() => NavigationActions.NavigateToSite(driver), 3, "[WARNING] Failed to navigate to site. Retrying...");
            Utilities.Retry(() => LoginActions.PerformLogin(driver), 3, "[WARNING] Login failed. Retrying...");

            Console.WriteLine("[SUCCESS] Logged into the site.");
        }

        internal static void SelectPatient(IWebDriver driver, Invoice invoice)
        {
            Console.WriteLine("[ACTION] Finding patient...");

            Utilities.Retry(() => NavigationActions.NavigateToMembershipSearch(driver), 3, "[WARNING] Failed to navigate to Membership Search. Retrying...");
            Utilities.Retry(() => PatientActions.FindPatient(driver, invoice), 3, "[WARNING] Failed to look up patient. Retrying...");
            Utilities.Retry(() => PatientActions.ChoosePatient(driver), 3, "[WARNING] Failed to select patient. Retrying...");

            Console.WriteLine($"[SUCCESS] Patient selected.");
        }

        internal static ServiceRequest GetServiceRequestWithAuthNumber(IWebDriver driver, string serviceDatesMonth)
        {
            Console.WriteLine("[ACTION] Selecting Service Request Authorization Number...");

            Utilities.Retry(() => NavigationActions.NavigateToAuthorizationRequests(driver), 3, "[WARNING] Failed to navigate to Authorization Requests. Retrying...");

            ServiceRequest serviceRequest = null;
            Utilities.Retry(() => serviceRequest = ServiceRequestActions.SelectServiceRequestWithAuthNumber(driver, serviceDatesMonth), 3, "[WARNING] Failed to get Claim. Retrying...");

            Console.WriteLine($"[INFO] Found Service Request Authorization Number: {serviceRequest?.SRAuth}");

            return serviceRequest;
        }

        internal static void ProcessData(IWebDriver driver, FormDataForProcessing formDataForProcessing)
        {
            Console.WriteLine("[ACTION] Processing service dates...");

            Utilities.Retry(() => NavigationActions.NavigateToAddClaims(driver), 3, "[WARNING] Failed to navigate to Add Claims. Retrying...");
            Utilities.Retry(() => ClaimsActions.CreateClaims(driver, formDataForProcessing), 3, "[WARNING] Failed to process claim. Retrying...");

            Console.WriteLine("[SUCCESS] Service dates processed successfully.");
        }
    }
}
