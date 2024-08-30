using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class ProcessClaimFormHeaderAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {
            Log.Information("[ACTION] Processing form header...");

            ClaimHeaderFormData formHeaderData = Context.Get<ClaimHeaderFormData>("FormHeaderData");
            var authorizationNumber = formHeaderData.AuthorizationNumber;
            var policyNumber = formHeaderData.PolicyNumber;
            var diagnosisCodes = formHeaderData.DiagnosisCodes;

            IWebElement medicaidCheckbox = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[label[contains(text(), 'MEDICAID')]]")));
            medicaidCheckbox.Click();
            Log.Information("Selected 'Medicaid' option.");

            IWebElement selfCheckbox = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[label[contains(text(), 'Self')]]")));
            selfCheckbox.Click();
            Log.Information("Selected 'Self' option.");

            IWebElement externalIDInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtInsuredID")));
            externalIDInput.SendKeys(Keys.Control + "a");
            externalIDInput.SendKeys(Keys.Delete);
            externalIDInput.SendKeys(policyNumber);
            Log.Information("Entered 'External ID'.");

            IWebElement signedInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthSign")));
            WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedInput, "Signature on File"));
            Log.Information("Verified 'Signature on File'.");

            IWebElement signedDate = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPatientAuthDate")));
            WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(signedDate, DateTime.Now.ToString("MM/dd/yyyy")));
            Log.Information("Verified signed date.");

            var indexedServiceDateRequests = diagnosisCodes.Select((diagnosisCode, index) => new { diagnosisCode, index });
            foreach (var indexedItem in indexedServiceDateRequests)
            {
                int index = indexedItem.index + 1;
                IWebElement diagnosisCodeInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDiagnosis" + index)));
                diagnosisCodeInput.SendKeys(Keys.Control + "a");
                diagnosisCodeInput.SendKeys(Keys.Delete);
                diagnosisCodeInput.SendKeys(indexedItem.diagnosisCode);
            }
            Log.Information("Entered diagnosis codes.");

            IWebElement authNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPriorAuthNumber")));
            authNumberInput.SendKeys(Keys.Control + "a");
            authNumberInput.SendKeys(Keys.Delete);
            authNumberInput.SendKeys(formHeaderData.AuthorizationNumber);
            Log.Information("Entered authorization number.");

            Log.Information("[SUCCESS] Processing form header.");
        }
    }
}
