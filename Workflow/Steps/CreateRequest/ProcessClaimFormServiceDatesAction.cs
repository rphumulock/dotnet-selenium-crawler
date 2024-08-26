using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class ProcessFormServiceDatesAction : WorkflowStepBase
    {
        protected List<ClaimServiceDateFormData> ServiceDatesFormData { get; init; }
        protected int BatchNumber { get; init; }

        internal ProcessFormServiceDatesAction(List<ClaimServiceDateFormData> serviceDatesFormData, int batchNumber)
        {
            ServiceDatesFormData = serviceDatesFormData;
            BatchNumber = batchNumber;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Processing form service dates...");

            try
            {
                var serviceDatesFormData = ServiceDatesFormData;
                int batchNumber = BatchNumber;

                var indexedServiceDateRequests = serviceDatesFormData.Select((serviceDate, index) => new { serviceDate, index });

                foreach (var indexedItem in indexedServiceDateRequests)
                {
                    int index = indexedItem.index + 1;

                    IWebElement dateInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDateOfServStart" + index)));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", dateInput);
                    dateInput.Click();
                    dateInput.SendKeys(Keys.Control + "a");
                    dateInput.SendKeys(Keys.Delete);
                    dateInput.SendKeys(indexedItem.serviceDate.StartDate);
                    Console.WriteLine($"[INFO] Entered service date for entry #{index} of batch #{batchNumber}.");

                    IWebElement dateInputDoneButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                    dateInputDoneButton.Click();
                    Console.WriteLine($"[INFO] Confirmed service date entry #{index} of batch #{batchNumber}.");

                    IWebElement placeOfServiceInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPlaceOfService" + index)));
                    placeOfServiceInput.Click();
                    placeOfServiceInput.SendKeys(Keys.Control + "a");
                    placeOfServiceInput.SendKeys(Keys.Delete);
                    placeOfServiceInput.SendKeys(indexedItem.serviceDate.PlaceOfService);
                    Console.WriteLine($"[INFO] Entered place of service for entry #{index} of batch #{batchNumber}.");

                    IWebElement cptInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCPT" + index)));
                    cptInput.Click();
                    WaitForModalToOpen(driver);
                    WaitForModalToClose(driver);
                    cptInput.SendKeys(Keys.Control + "a");
                    cptInput.SendKeys(Keys.Delete);
                    cptInput.SendKeys(indexedItem.serviceDate.CPT);
                    Console.WriteLine($"[INFO] Entered CPT code for entry #{index} of batch #{batchNumber}.");

                    IWebElement diagnosisInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosisPointer" + index)));
                    diagnosisInput.Click();
                    diagnosisInput.SendKeys(Keys.Control + "a");
                    diagnosisInput.SendKeys(Keys.Delete);
                    diagnosisInput.SendKeys(indexedItem.serviceDate.DiagnosisPointer);
                    Console.WriteLine($"[INFO] Entered diagnosis pointer for entry #{index} of batch #{batchNumber}.");

                    IWebElement chargesInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCharges" + index)));
                    chargesInput.Click();
                    chargesInput.SendKeys(Keys.Control + "a");
                    chargesInput.SendKeys(Keys.Delete);
                    chargesInput.SendKeys(indexedItem.serviceDate.ChargesDollars);
                    Console.WriteLine($"[INFO] Entered charges for entry #{index} of batch #{batchNumber}.");

                    IWebElement chargesCentsInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtChargescents" + index)));
                    chargesCentsInput.Click();
                    chargesCentsInput.SendKeys(Keys.Control + "a");
                    chargesCentsInput.SendKeys(Keys.Delete);
                    chargesCentsInput.SendKeys(indexedItem.serviceDate.ChargesCents);

                    IWebElement daysUnitsInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDaysUnits" + index)));
                    daysUnitsInput.Click();
                    daysUnitsInput.SendKeys(Keys.Control + "a");
                    daysUnitsInput.SendKeys(Keys.Delete);
                    daysUnitsInput.SendKeys(indexedItem.serviceDate.Units);
                    Console.WriteLine($"[INFO] Entered days/units for entry #{index} of batch #{batchNumber}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing service dates: {ex.Message}");
                throw;
            }
        }

        public void WaitForModalToOpen(IWebDriver driver)
        {
            WaitUntil(driver, d =>
            {
                var bodyElement = d.FindElement(By.TagName("body"));
                return bodyElement.GetAttribute("class").Contains("modal-open");
            });

            Console.WriteLine("[INFO] Modal opened.");
        }

        public void WaitForModalToClose(IWebDriver driver)
        {
            WaitUntil(driver, d =>
            {
                var bodyElement = d.FindElement(By.TagName("body"));
                return !bodyElement.GetAttribute("class").Contains("modal-open");
            });

            Console.WriteLine("[INFO] Modal closed.");
        }

    }
}

