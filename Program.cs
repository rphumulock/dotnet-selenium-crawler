using HAI_Selenium.Actions;
using HAISelenium.Actions;
using HAISelenium.InternalClasses;
using HAISelenium.Utils;
using OpenQA.Selenium;
using System;

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

                    // Prepare Data
                    Invoice invoice = Utilities.LoadJsonFile<Invoice>("Utils/request.json");
                    string serviceDatesMonth = ValidateServiceDateMonth(invoice);
                    PatientData patientData = new()
                    {
                        firstName = invoice.firstName,
                        lastName = invoice.lastName,
                        dob = invoice.dob,
                        policyNumber = invoice.policyNumber,
                        diagnosisCode = invoice.diagnosisCode,
                        providerID = invoice.providerID,
                        gender = invoice.gender,
                    };

                    // Prepare Incedo Data
                    LoginToSite(driver);
                    FindPatient(driver, patientData);
                    string srAuth = SelectServiceRequestAuthorizationNumber(driver, patientData, serviceDatesMonth);

                    // Process Claims
                    List<List<ServiceDateData>> batchedServiceDates = Utilities.BatchServiceDates(invoice.serviceDates, 6);
                    ProcessServiceDates(driver);

                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"An error occurred: {ex.Message}. Attempting retry {retryCount} of {maxRetries}...");

                    driver?.Close();
                    driver?.Quit();

                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("Max retries reached. Exiting program.");
                        throw;
                    }
                }
                finally
                {
                    driver?.Close();
                    driver?.Quit();
                    Console.WriteLine("Browser closed.");
                }
            }
        }

        internal static string ValidateServiceDateMonth(Invoice invoice)
        {
            string serviceDateMonth = null;
            foreach (var serviceDateData in invoice.serviceDates)
            {
                // Split the ServiceDate string by '/'
                string[] dateParts = serviceDateData.serviceDate.Split('/'); // dateParts[0] = month, dateParts[1] = day, dateParts[2] = year
                string currentMonth = dateParts[0];

                if (serviceDateMonth == null)
                {
                    serviceDateMonth = currentMonth;
                }
                else if (currentMonth != serviceDateMonth)
                {
                    throw new InvalidOperationException($"Mismatch found: expected month {serviceDateMonth}, but found {currentMonth}.");
                }
            }
            return serviceDateMonth;
        }

        internal static void LoginToSite(IWebDriver driver)
        {
            Utilities.Retry(() => NavigationActions.NavigateToSite(driver), 3, "Failed to navigate to site. Retrying...");
            Utilities.Retry(() => LoginActions.PerformLogin(driver), 3, "Login failed. Retrying...");
        }

        internal static void FindPatient(IWebDriver driver, PatientData patientData)
        {
            Utilities.Retry(() => NavigationActions.NavigateToMembershipSearch(driver), 3, "Failed to navigate to Membership Search. Retrying...");
            Utilities.Retry(() => PatientActions.FindPatient(driver, patientData), 3, "Failed to look up patient. Retrying...");
            Utilities.Retry(() => PatientActions.SelectPatient(driver), 3, "Failed to select patient. Retrying...");

            string externalID = null;
            Utilities.Retry(() => externalID = PatientActions.SelectPatientExternalID(driver), 3, "Failed to select patient external ID. Retrying...");
        }

        internal static string SelectServiceRequestAuthorizationNumber(IWebDriver driver, PatientData patientData, string serviceDatesMonth)
        {
            Utilities.Retry(() => NavigationActions.NavigateToAuthorizationRequests(driver), 3, "Failed to navigate to Authorization Requests. Retrying...");

            string srAuth = null;
            Utilities.Retry(() => srAuth = ServiceRequestActions.FindServiceRequestAuthorizationNumber(driver, serviceDatesMonth), 3, "Failed to get Claim. Retrying...");

            Console.WriteLine($"Found Service Request Authorization Number: {srAuth}");

            return srAuth;
        }

        internal static void ProcessServiceDates(IWebDriver driver)
        {
            Utilities.Retry(() => NavigationActions.NavigateToAddClaims(driver), 3, "Failed to navigate to Add Claims. Retrying...");
            Utilities.Retry(() => ClaimsActions.AddClaim(driver), 3, "Failed to navigate to Add Claims. Retrying...");
        }
    }
}
