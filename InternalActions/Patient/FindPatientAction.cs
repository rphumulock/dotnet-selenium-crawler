using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using HAI_Selenium.InternalClasses.Invoice;

internal class FindPatientAction : WorkflowStepBase
{
    protected WorkflowContext Context { get; init; }

    internal FindPatientAction(WorkflowContext context)
    {
        Context = context;
    }

    protected override void PerformStep(IWebDriver driver)
    {
        try
        {
            var invoice = Context.Get<RequestInvoice>("Invoice");

            Console.WriteLine($"[ACTION] Looking up patient {invoice.FirstName} {invoice.LastName}...");

            var policyNumber = invoice.PolicyNumber;
            if (!string.IsNullOrEmpty(policyNumber))
            {
                // Use the WaitUntil method from WorkflowStepBase to wait for the policy number input to be visible
                IWebElement policyNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPolicy")));
                policyNumberInput.SendKeys(policyNumber);
                Console.WriteLine("[INFO] Policy number added.");
            }
            else
            {
                if (!string.IsNullOrEmpty(invoice.FirstName))
                {
                    IWebElement firstNameInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFirst")));
                    firstNameInput.SendKeys(invoice.FirstName);
                    Console.WriteLine("[INFO] First name added.");
                }

                if (!string.IsNullOrEmpty(invoice.LastName))
                {
                    IWebElement lastNameInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtLast")));
                    lastNameInput.SendKeys(invoice.LastName);
                    Console.WriteLine("[INFO] Last name added.");
                }

                if (!string.IsNullOrEmpty(invoice.DateOfBirth))
                {
                    IWebElement birthDateInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDOB")));
                    birthDateInput.SendKeys(invoice.DateOfBirth);

                    IWebElement doneButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                    doneButton.Click();
                    Console.WriteLine("[INFO] Birthdate added.");
                }

                if (!string.IsNullOrEmpty(invoice.Gender))
                {
                    IWebElement genderDropdown = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ddGender")));
                    SelectElement selectElement = new SelectElement(genderDropdown);
                    selectElement.SelectByText(invoice.Gender);
                    Console.WriteLine("[INFO] Gender selected.");
                }
            }

            IWebElement searchButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1")));
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
