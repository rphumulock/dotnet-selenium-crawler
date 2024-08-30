using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class FindPatientAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override void PerformStep(IWebDriver driver)
        {

            InvoiceRequest createClaimsRequest = Context.Get<InvoiceRequest>("InvoiceRequest");

            Log.Information("[ACTION] Initiating patient lookup for {FirstName} {LastName}...", createClaimsRequest.FirstName, createClaimsRequest.LastName);

            var policyNumber = createClaimsRequest.PolicyNumber;
            if (!string.IsNullOrEmpty(policyNumber))
            {
                LogInputAction(driver, "txtPolicy", policyNumber, "[INFO] Policy number added.");
            }
            else
            {
                if (!string.IsNullOrEmpty(createClaimsRequest.FirstName))
                {
                    LogInputAction(driver, "txtFirst", createClaimsRequest.FirstName, "[INFO] First name added.");
                }

                if (!string.IsNullOrEmpty(createClaimsRequest.LastName))
                {
                    LogInputAction(driver, "txtLast", createClaimsRequest.LastName, "[INFO] Last name added.");
                }

                if (!string.IsNullOrEmpty(createClaimsRequest.DateOfBirth))
                {
                    LogInputAction(driver, "txtDOB", createClaimsRequest.DateOfBirth, "[INFO] Birthdate added.", clickAfterInputSelector: "button.ui-datepicker-close[data-handler='hide'][data-event='click']");
                }

                if (!string.IsNullOrEmpty(createClaimsRequest.Gender))
                {
                    SelectGender(driver, "ddGender", createClaimsRequest.Gender);
                }
            }

            ClickSearchButton(driver);

            Log.Information("[SUCCESS] Looking up patient {FirstName} {LastName}...", createClaimsRequest.FirstName, createClaimsRequest.LastName);
        }

        private void LogInputAction(IWebDriver driver, string elementId, string inputValue, string successMessage, string clickAfterInputSelector = null)
        {
            IWebElement inputElement = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(elementId)));
            inputElement.SendKeys(inputValue);

            if (clickAfterInputSelector != null)
            {
                IWebElement doneButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(clickAfterInputSelector)));
                doneButton.Click();
            }

            Log.Information(successMessage);
        }

        private void SelectGender(IWebDriver driver, string elementId, string gender)
        {
            IWebElement genderDropdown = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(elementId)));
            SelectElement selectElement = new SelectElement(genderDropdown);
            selectElement.SelectByText(gender);
        }

        private void ClickSearchButton(IWebDriver driver)
        {
            IWebElement searchButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
            searchButton.Click();
            Log.Information("Search button clicked successfully.");
        }
    }
}
