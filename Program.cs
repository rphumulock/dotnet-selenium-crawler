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
                    Console.WriteLine("[ACTION] Loading environment variables...");
                    Utilities.LoadEnvVariables();
                    Console.WriteLine("[SUCCESS] Environment variables loaded.");

                    Console.WriteLine("[ACTION] Logging current user info...");
                    Utilities.LogCurrentUserInfo();
                    Console.WriteLine("[SUCCESS] User info logged.");

                    Console.WriteLine("[ACTION] Setting up WebDriver...");
                    driver = Utilities.SetupDriver();
                    Console.WriteLine("[SUCCESS] WebDriver setup complete.");

                    // Prepare Data
                    Console.WriteLine("[ACTION] Loading invoice data...");
                    Invoice invoice = Utilities.LoadJsonFile<Invoice>("Utils/request.json");
                    string serviceDatesMonth = ValidateServiceDateMonth(invoice);
                    Console.WriteLine($"[SUCCESS] Invoice data loaded and service month validated: {serviceDatesMonth}.");

                    PatientData patientData = new()
                    {
                        firstName = invoice.firstName,
                        lastName = invoice.lastName,
                        dob = invoice.dob,
                        policyNumber = invoice.policyNumber,
                        diagnosisCode = invoice.diagnosisCode.Split('.')[0], // Remove the decimal part of the diagnosis code
                        providerID = invoice.providerID,
                        gender = invoice.gender,
                    };
                    Console.WriteLine("[INFO] Patient data prepared.");

                    // Prepare Incedo Data
                    Console.WriteLine("[ACTION] Logging into the site...");
                    LoginToSite(driver);
                    Console.WriteLine("[SUCCESS] Logged into the site.");

                    Console.WriteLine("[ACTION] Finding patient...");
                    string externalID = FindPatient(driver, patientData);
                    Console.WriteLine($"[SUCCESS] Patient found with External ID: {externalID}.");

                    Console.WriteLine("[ACTION] Selecting Service Request Authorization Number...");
                    string srAuth = SelectServiceRequestAuthorizationNumber(driver, patientData, serviceDatesMonth);
                    Console.WriteLine($"[SUCCESS] Service Request Authorization Number selected: {srAuth}.");

                    // Process Claims
                    Console.WriteLine("[ACTION] Processing service dates...");
                    List<List<ServiceDateData>> batchedServiceDates = Utilities.BatchServiceDates(invoice.serviceDates, 6);
                    ProcessServiceDates(driver, patientData, batchedServiceDates, externalID, srAuth);
                    Console.WriteLine("[SUCCESS] Service dates processed successfully.");

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
                finally
                {
                    Console.WriteLine("[INFO] Closing browser...");
                    driver?.Close();
                    driver?.Quit();
                    Console.WriteLine("[INFO] Browser closed.");
                }
            }
        }

        internal static string ValidateServiceDateMonth(Invoice invoice)
        {
            string serviceDateMonth = null;
            foreach (var serviceDateData in invoice.serviceDates)
            {
                string[] dateParts = serviceDateData.serviceDate.Split('/');
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
            return serviceDateMonth;
        }

        internal static void LoginToSite(IWebDriver driver)
        {
            Utilities.Retry(() => NavigationActions.NavigateToSite(driver), 3, "[WARNING] Failed to navigate to site. Retrying...");
            Utilities.Retry(() => LoginActions.PerformLogin(driver), 3, "[WARNING] Login failed. Retrying...");
        }

        internal static string FindPatient(IWebDriver driver, PatientData patientData)
        {
            Utilities.Retry(() => NavigationActions.NavigateToMembershipSearch(driver), 3, "[WARNING] Failed to navigate to Membership Search. Retrying...");
            Utilities.Retry(() => PatientActions.FindPatient(driver, patientData), 3, "[WARNING] Failed to look up patient. Retrying...");
            Utilities.Retry(() => PatientActions.SelectPatient(driver), 3, "[WARNING] Failed to select patient. Retrying...");

            string externalID = null;
            Utilities.Retry(() => externalID = PatientActions.SelectPatientExternalID(driver), 3, "[WARNING] Failed to select patient external ID. Retrying...");
            return externalID;
        }

        internal static string SelectServiceRequestAuthorizationNumber(IWebDriver driver, PatientData patientData, string serviceDatesMonth)
        {
            Utilities.Retry(() => NavigationActions.NavigateToAuthorizationRequests(driver), 3, "[WARNING] Failed to navigate to Authorization Requests. Retrying...");

            string srAuth = null;
            Utilities.Retry(() => srAuth = ServiceRequestActions.FindServiceRequestAuthorizationNumber(driver, serviceDatesMonth), 3, "[WARNING] Failed to get Claim. Retrying...");

            Console.WriteLine($"[INFO] Found Service Request Authorization Number: {srAuth}");

            return srAuth;
        }

        internal static void ProcessServiceDates(IWebDriver driver, PatientData patientData, List<List<ServiceDateData>> batchedServiceDates, string externalID, string srAuth)
        {
            Utilities.Retry(() => NavigationActions.NavigateToAddClaims(driver), 3, "[WARNING] Failed to navigate to Add Claims. Retrying...");

            var indexedBatchedServiceDates = batchedServiceDates.Select((batch, index) => new { batch, index });

            foreach (var item in indexedBatchedServiceDates)
            {
                var batchNumber = item.index + 1;
                var batchCount = item.batch.Count;
                Utilities.Retry(() => ClaimsActions.ProcessClaim(driver, patientData, item.batch, batchCount, batchNumber, externalID, srAuth), 3, "[WARNING] Failed to process claim. Retrying...");
            }
        }
    }
}
