using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class ProcessFormServiceDatesAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Processing form service dates...");

            List<ClaimServiceDateFormData> currentBatch = Context.Get<List<ClaimServiceDateFormData>>("CurrentBatchServiceDateFormData");
            var indexedServiceDateRequests = currentBatch.Select((serviceDate, index) => new { serviceDate, index });

            foreach (var indexedItem in indexedServiceDateRequests)
            {
                int index = indexedItem.index + 1;

                IWebElement dateInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDateOfServStart" + index)));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", dateInput);
                dateInput.Click();
                dateInput.SendKeys(Keys.Control + "a");
                dateInput.SendKeys(Keys.Delete);
                dateInput.SendKeys(indexedItem.serviceDate.StartDate);
                Log.Information("Entered service date for entry #{Index} of batch.", index);

                IWebElement dateInputDoneButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                dateInputDoneButton.Click();
                Log.Information("Confirmed service date entry #{Index} of batch.", index);

                IWebElement placeOfServiceInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPlaceOfService" + index)));
                placeOfServiceInput.Click();
                placeOfServiceInput.SendKeys(Keys.Control + "a");
                placeOfServiceInput.SendKeys(Keys.Delete);
                placeOfServiceInput.SendKeys(indexedItem.serviceDate.PlaceOfService);
                Log.Information("Entered place of service for entry #{Index} of batch.", index);

                IWebElement cptInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCPT" + index)));
                cptInput.Click();
                WaitForModalToOpen(driver);
                WaitForModalToClose(driver);
                cptInput.SendKeys(Keys.Control + "a");
                cptInput.SendKeys(Keys.Delete);
                cptInput.SendKeys(indexedItem.serviceDate.CPT);
                Log.Information("Entered CPT code for entry #{Index} of batch.", index);

                IWebElement diagnosisInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosisPointer" + index)));
                diagnosisInput.Click();
                diagnosisInput.SendKeys(Keys.Control + "a");
                diagnosisInput.SendKeys(Keys.Delete);
                diagnosisInput.SendKeys(indexedItem.serviceDate.DiagnosisPointer);
                Log.Information("Entered diagnosis pointer for entry #{Index} of batch.", index);

                IWebElement chargesInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtCharges" + index)));
                chargesInput.Click();
                chargesInput.SendKeys(Keys.Control + "a");
                chargesInput.SendKeys(Keys.Delete);
                chargesInput.SendKeys(indexedItem.serviceDate.ChargesDollars);
                Log.Information("Entered charges for entry #{Index} of batch.", index);

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
                Log.Information("Entered days/units for entry #{Index} of batch.", index);
            }
            
            Log.Information("[SUCCESS] Processing form service dates.");
        }

        public void WaitForModalToOpen(IWebDriver driver)
        {
            WaitUntil(driver, d =>
            {
                var bodyElement = d.FindElement(By.TagName("body"));
                return bodyElement.GetAttribute("class").Contains("modal-open");
            });

            Log.Information("Modal opened.");
        }

        public void WaitForModalToClose(IWebDriver driver)
        {
            WaitUntil(driver, d =>
            {
                var bodyElement = d.FindElement(By.TagName("body"));
                return !bodyElement.GetAttribute("class").Contains("modal-open");
            });

            Log.Information("Modal closed.");
        }
    }
}
