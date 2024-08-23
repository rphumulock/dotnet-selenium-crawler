using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using HAI_Selenium.InternalClasses.Invoice;

internal class FindPatientAction : WorkflowStepBase
{
    private readonly RequestInvoice _invoice;

    public FindPatientAction(RequestInvoice invoice)
    {
        _invoice = invoice;
    }

    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine($"[ACTION] Looking up patient {_invoice.FirstName} {_invoice.LastName}...");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            string policyNumber = _invoice.PolicyNumber;
            if (!string.IsNullOrEmpty(policyNumber))
            {
                IWebElement policyNumberInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPolicy")));
                policyNumberInput.SendKeys(policyNumber);
                Console.WriteLine("[INFO] Policy number added.");
            }
            else
            {
                if (!string.IsNullOrEmpty(_invoice.FirstName))
                {
                    IWebElement firstNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFirst")));
                    firstNameInput.SendKeys(_invoice.FirstName);
                    Console.WriteLine("[INFO] First name added.");
                }

                if (!string.IsNullOrEmpty(_invoice.LastName))
                {
                    IWebElement lastNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtLast")));
                    lastNameInput.SendKeys(_invoice.LastName);
                    Console.WriteLine("[INFO] Last name added.");
                }

                if (!string.IsNullOrEmpty(_invoice.DateOfBirth))
                {
                    IWebElement birthDateInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDOB")));
                    birthDateInput.SendKeys(_invoice.DateOfBirth);

                    IWebElement doneButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                    doneButton.Click();
                    Console.WriteLine("[INFO] Birthdate added.");
                }

                if (!string.IsNullOrEmpty(_invoice.Gender))
                {
                    IWebElement genderDropdown = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ddGender")));
                    SelectElement selectElement = new SelectElement(genderDropdown);
                    selectElement.SelectByText(_invoice.Gender);
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
}
