using OpenQA.Selenium;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Utilities;

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
                var authorizationNumber = formHeaderData.AuthorizationNumber;
                var policyNumber = formHeaderData.PolicyNumber;
                var diagnosisCodes = formHeaderData.DiagnosisCodes;

                IWebElement medicaidCheckbox = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[label[contains(text(), 'MEDICAID')]]")));
                medicaidCheckbox.Click();

                Console.WriteLine("[INFO] Selected 'Medicaid' option.");

                IWebElement selfCheckbox = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[label[contains(text(), 'Self')]]")));
                selfCheckbox.Click();

                Console.WriteLine("[INFO] Selected 'Self' option.");

                IWebElement externalIDInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtInsuredID")));
                externalIDInput.SendKeys(Keys.Control + "a");
                externalIDInput.SendKeys(Keys.Delete);
                externalIDInput.SendKeys(policyNumber);

                Console.WriteLine("[INFO] Entered 'External ID'.");

                IWebElement signedInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthSign")));
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedInput, "Signature on File"));

                Console.WriteLine("[INFO] Verified 'Signature on File'.");

                IWebElement signedDate = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthDate")));
                WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedDate, DateTime.Now.ToString("MM/dd/yyyy")));

                Console.WriteLine("[INFO] Verified signed date.");


                try
                {
                    var indexedServiceDateRequests = diagnosisCodes.Select((diagnosisCode, index) => new { diagnosisCode, index });
                    foreach (var indexedItem in indexedServiceDateRequests)
                    {
                        int index = indexedItem.index + 1;
                        IWebElement diagnosisCodeInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosis" + index)));
                        diagnosisCodeInput.SendKeys(Keys.Control + "a");
                        diagnosisCodeInput.SendKeys(Keys.Delete);
                        diagnosisCodeInput.SendKeys(indexedItem.diagnosisCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");

                    throw new NonRecoverableError(ex.Message, ex);
                }

                Console.WriteLine("[INFO] Entered diagnosis codes.");

                IWebElement authNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPriorAuthNumber")));
                authNumberInput.SendKeys(Keys.Control + "a");
                authNumberInput.SendKeys(Keys.Delete);
                authNumberInput.SendKeys(formHeaderData.AuthorizationNumber);

                Console.WriteLine("[INFO] Entered authorization number.");
            }
            catch (RecoverableError ex)
            {
                Console.WriteLine($"[ERROR] Recoverable error occurred while processing form: {ex.Message}");
                throw new RecoverableError(ex.Message, ex);
            }
            catch (NonRecoverableError ex)
            {
                Console.WriteLine($"[ERROR] Non-Recoverable error occurred while processing form: {ex.Message}");
                throw new NonRecoverableError(ex.Message, ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An unexpected error occurred while processing form: {ex.Message}");
                throw;
            }
        }
    }
}