using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class ProcessClaimFormHeaderAction : WorkflowStepBase
    {

        protected WorkflowContext Context { get; init; }

        internal ProcessClaimFormHeaderAction(WorkflowContext context)
        {
            Context = context;
        }

        protected override void PerformStep(IWebDriver driver)
        {
            Console.WriteLine("[ACTION] Processing form header...");

            try
            {
                ClaimHeaderFormData formHeaderData = Context.Get<ClaimHeaderFormData>("FormHeaderData");

                IWebElement medicaidCheckbox = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[label[contains(text(), 'MEDICAID')]]")));
                medicaidCheckbox.Click();

                Console.WriteLine("[INFO] Selected 'Medicaid' option.");

                IWebElement selfCheckbox = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[label[contains(text(), 'Self')]]")));
                selfCheckbox.Click();

                Console.WriteLine("[INFO] Selected 'Self' option.");

                IWebElement externalIDInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtInsuredID")));
                externalIDInput.SendKeys(Keys.Control + "a");
                externalIDInput.SendKeys(Keys.Delete);
                externalIDInput.SendKeys(formHeaderData.PolicyNumber);

                Console.WriteLine("[INFO] Entered 'External ID'.");

                IWebElement signedInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthSign")));
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedInput, "Signature on File"));

                Console.WriteLine("[INFO] Verified 'Signature on File'.");

                IWebElement signedDate = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthDate")));
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedDate, DateTime.Now.ToString("MM/dd/yyyy")));

                Console.WriteLine("[INFO] Verified signed date.");

                var indexedServiceDateRequests = formHeaderData.DiagnosisCodes.Select((diagnosisCode, index) => new { diagnosisCode, index });
                foreach (var indexedItem in indexedServiceDateRequests)
                {
                    int index = indexedItem.index + 1;
                    IWebElement diagnosisCodeInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosis" + index)));
                    diagnosisCodeInput.SendKeys(Keys.Control + "a");
                    diagnosisCodeInput.SendKeys(Keys.Delete);
                    diagnosisCodeInput.SendKeys(indexedItem.diagnosisCode);
                }
                Console.WriteLine("[INFO] Entered diagnosis codes.");

                IWebElement authNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPriorAuthNumber")));
                authNumberInput.SendKeys(Keys.Control + "a");
                authNumberInput.SendKeys(Keys.Delete);
                authNumberInput.SendKeys(formHeaderData.AuthorizationNumber);

                Console.WriteLine("[INFO] Entered authorization number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");
                throw;
            }
        }
    }
}