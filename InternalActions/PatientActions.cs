using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAI_Selenium.InternalClasses;
using HAI_Selenium.Utils;

namespace HAI_Selenium.InternalActions
{
    internal class PatientActions
    {
        internal static void SelectPatient(IWebDriver driver, Invoice invoice)
        {
            Console.WriteLine("[ACTION] Finding patient...");

            Utilities.Retry(() => NavigationActions.NavigateToMembershipSearch(driver), 3, "[WARNING] Failed to navigate to Membership Search. Retrying...");
            Utilities.Retry(() => FindPatient(driver, invoice), 3, "[WARNING] Failed to look up patient. Retrying...");
            Utilities.Retry(() => ChoosePatient(driver), 3, "[WARNING] Failed to select patient. Retrying...");

            Console.WriteLine($"[SUCCESS] Patient selected.");
        }

        internal static void FindPatient(IWebDriver driver, Invoice invoice)
        {
            Console.WriteLine($"[ACTION] Looking up patient {invoice.FirstName} {invoice.LastName}...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            try
            {
                string policyNumber = invoice.PolicyNumber;
                if (!string.IsNullOrEmpty(policyNumber))
                {
                    IWebElement policyNumberInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPolicy")));
                    policyNumberInput.SendKeys(policyNumber);
                    Console.WriteLine("[INFO] Policy number added.");
                }
                else
                {
                    if (!string.IsNullOrEmpty(invoice.FirstName))
                    {
                        IWebElement firstNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFirst")));
                        firstNameInput.SendKeys(invoice.FirstName);
                        Console.WriteLine("[INFO] First name added.");
                    }

                    if (!string.IsNullOrEmpty(invoice.LastName))
                    {
                        IWebElement lastNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtLast")));
                        lastNameInput.SendKeys(invoice.LastName);
                        Console.WriteLine("[INFO] Last name added.");
                    }

                    if (!string.IsNullOrEmpty(invoice.DateOfBirth))
                    {
                        IWebElement birthDateInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDOB")));
                        birthDateInput.SendKeys(invoice.DateOfBirth);

                        IWebElement doneButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                        doneButton.Click();
                        Console.WriteLine("[INFO] Birthdate added.");
                    }

                    if (!string.IsNullOrEmpty(invoice.Gender))
                    {
                        IWebElement genderDropdown = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ddGender")));
                        SelectElement selectElement = new SelectElement(genderDropdown);
                        selectElement.SelectByText(invoice.Gender);
                        Console.WriteLine("[INFO] Gender selected.");
                    }
                }

                IWebElement searchButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
                searchButton.Click();
                Console.WriteLine("[INFO] Search button clicked successfully.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"[ERROR] Timeout while finding patient: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while finding patient: {ex.Message}");
                throw;
            }
        }

        internal static void ChoosePatient(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Selecting patient...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            try
            {
                IWebElement patientGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrid")));
                patientGrid.Click();

                Console.WriteLine("[INFO] Patient selected successfully.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"[ERROR] Timeout while selecting patient: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while selecting patient: {ex.Message}");
                throw;
            }
        }

        internal static string SelectPatientExternalID(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Selecting patient ExternalID...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            try
            {
                IWebElement externalID = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("lblExtId")));
                string externalIDText = externalID.Text;

                Console.WriteLine($"[INFO] Patient External ID: {externalIDText}");
                return externalIDText;
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"[ERROR] Timeout while selecting patient ExternalID: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while selecting patient ExternalID: {ex.Message}");
                throw;
            }
        }
    }
}
