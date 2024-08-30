using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
    internal class FindPatientAction(WorkflowContext context) : WorkflowStepBase(context)
    {
        protected override Task PerformStepAsync(IWebDriver driver)
        {

            InvoiceRequest mockRequest = Context.Get<InvoiceRequest>("MockRequest");

            Log.Information("[ACTION] Initiating patient lookup for {FirstName} {LastName}...", mockRequest.FirstName, mockRequest.LastName);

            var policyNumber = mockRequest.PolicyNumber;
            if (!string.IsNullOrEmpty(policyNumber))
            {
                LogInputAction(driver, "txtPolicy", policyNumber, "[INFO] Policy number added.");
            }
            else
            {
                if (!string.IsNullOrEmpty(mockRequest.FirstName))
                {
                    LogInputAction(driver, "txtFirst", mockRequest.FirstName, "[INFO] First name added.");
                }

                if (!string.IsNullOrEmpty(mockRequest.LastName))
                {
                    LogInputAction(driver, "txtLast", mockRequest.LastName, "[INFO] Last name added.");
                }

                if (!string.IsNullOrEmpty(mockRequest.DateOfBirth))
                {
                    LogInputAction(driver, "txtDOB", mockRequest.DateOfBirth, "[INFO] Birthdate added.", clickAfterInputSelector: "button.ui-datepicker-close[data-handler='hide'][data-event='click']");
                }

                if (!string.IsNullOrEmpty(mockRequest.Gender))
                {
                    SelectGender(driver, "ddGender", mockRequest.Gender);
                }
            }

            ClickSearchButton(driver);

            Log.Information("[SUCCESS] Looking up patient {FirstName} {LastName}...", mockRequest.FirstName, mockRequest.LastName);

            return Task.CompletedTask;
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
