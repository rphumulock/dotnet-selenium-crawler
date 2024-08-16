using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.InternalClasses;

namespace HAISelenium.Actions
{
    internal class PatientActions
    {
        internal static void FindPatient(IWebDriver driver, PatientData patientData)
        {
            Console.WriteLine($"Looking up patient {patientData.firstName} {patientData.lastName} ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            string policyNumber = patientData.policyNumber;
            if (!string.IsNullOrEmpty(policyNumber))
            {
                IWebElement policyNumberInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPolicy")));
                policyNumberInput.SendKeys(policyNumber);
                Console.WriteLine("First Name added.");
            }
            else
            {
                string firstname = patientData.firstName;
                string lastname = patientData.lastName;
                string DoB = patientData.dob;
                string gender = patientData.gender;

                if (!string.IsNullOrEmpty(firstname))
                {
                    IWebElement firstNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFirst")));
                    firstNameInput.SendKeys(firstname);
                    Console.WriteLine("First Name added.");
                }

                if (!string.IsNullOrEmpty(lastname))
                {
                    IWebElement lastNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtLast")));
                    lastNameInput.SendKeys(lastname);
                    Console.WriteLine("Last Name added.");
                }

                if (!string.IsNullOrEmpty(DoB))
                {
                    IWebElement birthDateInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDOB")));
                    birthDateInput.SendKeys(DoB);

                    IWebElement doneButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                    doneButton.Click();
                    Console.WriteLine("Birthday added.");
                }

                if (!string.IsNullOrEmpty(gender))
                {
                    IWebElement genderDropdown = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ddGender")));
                    SelectElement selectElement = new SelectElement(genderDropdown);
                    selectElement.SelectByText(gender);
                    Console.WriteLine("Gender selected.");
                }
            }


            IWebElement searchButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
            searchButton.Click();
            Console.WriteLine("Search button clicked successfully.");
        }

        internal static void SelectPatient(IWebDriver driver)
        {
            Console.WriteLine("Selecting patient ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement patientGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrid")));
            patientGrid.Click();
            Console.WriteLine("Patient selected successfully.");
        }

        internal static string SelectPatientExternalID(IWebDriver driver)
        {
            Console.WriteLine("Seleting patient ExternalID ...");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement externalID = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("lblExtId")));
            string externalIDText = externalID.Text;

            Console.WriteLine("Patient External ID: " + externalIDText);
            return externalIDText;
        }
    }
}
