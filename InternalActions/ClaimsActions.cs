using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using HAISelenium.InternalClasses;
using HAISelenium.Utils;
using HAI_Selenium.InternalClasses;

namespace HAISelenium.InternalActions
{
    internal class ClaimsActions
    {
        internal static void CreateClaims(IWebDriver driver, FormDataForProcessing formDataForProcessing)
        {
            Console.WriteLine($"[ACTION] Creating Claims...");

            var indexedBatchedServiceDates = formDataForProcessing.serviceDatesFormData.Select((serviceDatesBatch, index) => new { serviceDatesBatch, index }).ToList();
            foreach (var indexItem in indexedBatchedServiceDates)
            {
                var isLastBatch = indexItem.index == indexedBatchedServiceDates.Count - 1;
                var batchNumber = indexItem.index + 1;
                var batchCount = indexItem.serviceDatesBatch.Count;

                Console.WriteLine($"[ACTION] Processing Claim for batch #{batchNumber}...");
                Utilities.Retry(() => ProcessForm(driver, formDataForProcessing.patientFormData, indexItem.serviceDatesBatch, batchNumber, isLastBatch), 3, "[WARNING] Failed to process service dates. Retrying...");
            }
        }

        internal static void ProcessForm(IWebDriver driver, PatientFormData patientFormData, List<ServiceDateFormData> serviceDates, int batchNumber, bool isLastBatch)
        {

            var wait = Utilities.CreateWebDriverWait(driver);

            IWebElement addButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
            IWebElement cancelButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran3")));

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(addButton));
            addButton.Click();
            Console.WriteLine("[INFO] Clicked 'Add' button.");

            ProcessFormHeader(driver, patientFormData);
            ProcessFormServiceDates(driver, serviceDates, batchNumber);
            ProcessFormFooter(driver);

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", cancelButton);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(cancelButton));
            if (!isLastBatch)
            {
                cancelButton.Click();

            }
            Console.WriteLine("[SUCCESS] Claim processed.");
        }

        internal static void ProcessFormHeader(IWebDriver driver, PatientFormData patientFormData)
        {
            var wait = Utilities.CreateWebDriverWait(driver);

            ClickElement(driver, wait, By.XPath("//div[label[contains(text(), 'MEDICAID')]]"));
            Console.WriteLine("[INFO] Selected 'Medicaid' option.");

            ClickElement(driver, wait, By.XPath("//div[label[contains(text(), 'Self')]]"));
            Console.WriteLine("[INFO] Selected 'Self' option.");

            EnterText(driver, wait, By.Id("txtInsuredID"), patientFormData.patientPolicyNumber);
            Console.WriteLine("[INFO] Entered 'External ID'.");

            IWebElement signedInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthSign")));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedInput, "Signature on File"));
            Console.WriteLine("[INFO] Verified 'Signature on File'.");

            IWebElement signedDate = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthDate")));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedDate, DateTime.Now.ToString("MM/dd/yyyy")));
            Console.WriteLine("[INFO] Verified signed date.");

            IWebElement diagnosisCodeInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosis1")));
            diagnosisCodeInput.Clear();
            diagnosisCodeInput.SendKeys(patientFormData.patientDiagnosisCode);
            Console.WriteLine("[INFO] Entered diagnosis code.");

            IWebElement authNumberInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPriorAuthNumber")));
            authNumberInput.Clear();
            authNumberInput.SendKeys(patientFormData.authNumber);
            Console.WriteLine("[INFO] Entered authorization number.");
        }

        internal static void ProcessFormServiceDates(IWebDriver driver, List<ServiceDateFormData> serviceDatesFormData, int batchNumber)
        {
            var wait = Utilities.CreateWebDriverWait(driver);

            var indexedServiceDateRequests = serviceDatesFormData.Select((serviceDate, index) => new { serviceDate, index });
            foreach (var indexedItem in indexedServiceDateRequests)
            {
                int index = indexedItem.index + 1;

                IWebElement dateInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDateOfServStart" + index)));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", dateInput);
                dateInput.Click();
                dateInput.Clear();
                dateInput.SendKeys(indexedItem.serviceDate.StartDate);
                Console.WriteLine($"[INFO] Entered service date for entry #{index} of batch #{batchNumber}.");
                IWebElement dateInputDoneButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                dateInputDoneButton.Click();
                Console.WriteLine($"[INFO] Confirmed service date entry #{index} of batch #{batchNumber}.");

                IWebElement placeOfServiceInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPlaceOfService" + index)));
                placeOfServiceInput.Click();
                placeOfServiceInput.Clear();
                placeOfServiceInput.SendKeys(indexedItem.serviceDate.PlaceOfService);
                Console.WriteLine($"[INFO] Entered place of service for entry #{index} of batch #{batchNumber}.");

                IWebElement cptInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCPT" + index)));
                cptInput.Click();
                WaitForModalToOpen(driver, 20);
                WaitForModalToClose(driver, 20);
                cptInput.Clear();
                cptInput.SendKeys(indexedItem.serviceDate.CPT);
                Console.WriteLine($"[INFO] Entered CPT code for entry #{index} of batch #{batchNumber}.");

                IWebElement diagnosisInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosisPointer" + index)));
                diagnosisInput.Click();
                diagnosisInput.Clear();
                diagnosisInput.SendKeys(indexedItem.serviceDate.DiagnosisPointer);
                Console.WriteLine($"[INFO] Entered diagnosis pointer for entry #{index} of batch #{batchNumber}.");

                IWebElement chargesInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCharges" + index)));
                chargesInput.Click();
                chargesInput.Clear();
                chargesInput.SendKeys(indexedItem.serviceDate.ChargesDollars);
                Console.WriteLine($"[INFO] Entered charges for entry #{index} of batch #{batchNumber}.");

                IWebElement chargesCentsInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtChargescents" + index)));
                chargesCentsInput.Click();
                chargesCentsInput.Clear();
                chargesCentsInput.SendKeys(indexedItem.serviceDate.ChargesCents);

                IWebElement daysUnitsInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDaysUnits" + index)));
                daysUnitsInput.Click();
                daysUnitsInput.Clear();
                daysUnitsInput.SendKeys(indexedItem.serviceDate.Units);
                Console.WriteLine($"[INFO] Entered days/units for entry #{index} of batch #{batchNumber}.");
            }
        }

        internal static void ProcessFormFooter(IWebDriver driver)
        {
            var wait = Utilities.CreateWebDriverWait(driver);

            IWebElement einNumberInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFedTaxID")));
            wait.Until(driver => !string.IsNullOrEmpty(einNumberInput.GetAttribute("value")));
            Console.WriteLine("[INFO] Verified EIN number.");

            IWebElement physPhoneInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPhysPhone")));
            wait.Until(driver => !string.IsNullOrEmpty(physPhoneInput.GetAttribute("value")));
            Console.WriteLine("[INFO] Verified physician's phone number.");

            IWebElement physSignedDateInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPhysicianSignedDate")));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(physSignedDateInput, DateTime.Now.ToString("MM/dd/yyyy")));
            Console.WriteLine("[INFO] Verified physician's signed date.");
        }

        // HELPERS
        private static void ClickElement(IWebDriver driver, WebDriverWait wait, By by)
        {
            IWebElement element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
            element.Click();
            Console.WriteLine($"[INFO] Clicked element located by {by}.");
        }

        private static void EnterText(IWebDriver driver, WebDriverWait wait, By by, string text)
        {
            IWebElement element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
            element.Clear();
            element.SendKeys(text);
            Console.WriteLine($"[INFO] Entered text '{text}' into element located by {by}.");
        }

        internal static void AddClaim(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Adding Claim...");
            var wait = Utilities.CreateWebDriverWait(driver);

            ClickElement(driver, wait, By.CssSelector("button#tran1"));
            Console.WriteLine("[SUCCESS] Claim added.");
        }

        internal static void CancelClaim(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Canceling Claim...");
            var wait = Utilities.CreateWebDriverWait(driver);

            ClickElement(driver, wait, By.CssSelector("button#tran3"));
            Console.WriteLine("[SUCCESS] Claim canceled.");
        }


        // Method to wait until the body has the 'modal-open' class
        public static void WaitForModalToOpen(IWebDriver driver, int timeoutInSeconds)
        {
            var wait = Utilities.CreateWebDriverWait(driver);
            wait.Until(d =>
            {
                var bodyElement = d.FindElement(By.TagName("body"));
                return bodyElement.GetAttribute("class").Contains("modal-open");
            });
            Console.WriteLine("[INFO] Modal opened.");
        }

        // Method to wait until the body no longer has the 'modal-open' class
        public static void WaitForModalToClose(IWebDriver driver, int timeoutInSeconds)
        {
            var wait = Utilities.CreateWebDriverWait(driver);
            wait.Until(d =>
            {
                var bodyElement = d.FindElement(By.TagName("body"));
                return !bodyElement.GetAttribute("class").Contains("modal-open");
            });
            Console.WriteLine("[INFO] Modal closed.");
        }

    }
}
