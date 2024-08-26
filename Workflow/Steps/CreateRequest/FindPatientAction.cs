using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps.CreateRequest
{
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
                CreateClaimsRequest createClaimsRequest = Context.Get<CreateClaimsRequest>("CreateClaimsRequest");

                Console.WriteLine($"[ACTION] Looking up patient {createClaimsRequest.FirstName} {createClaimsRequest.LastName}...");

                var policyNumber = createClaimsRequest.PolicyNumber;
                if (!string.IsNullOrEmpty(policyNumber))
                {
                    // Use the WaitUntil method from WorkflowStepBase to wait for the policy number input to be visible
                    IWebElement policyNumberInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtPolicy")));
                    policyNumberInput.SendKeys(policyNumber);
                    Console.WriteLine("[INFO] Policy number added.");
                }
                else
                {
                    if (!string.IsNullOrEmpty(createClaimsRequest.FirstName))
                    {
                        IWebElement firstNameInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFirst")));
                        firstNameInput.SendKeys(createClaimsRequest.FirstName);
                        Console.WriteLine("[INFO] First name added.");
                    }

                    if (!string.IsNullOrEmpty(createClaimsRequest.LastName))
                    {
                        IWebElement lastNameInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtLast")));
                        lastNameInput.SendKeys(createClaimsRequest.LastName);
                        Console.WriteLine("[INFO] Last name added.");
                    }

                    if (!string.IsNullOrEmpty(createClaimsRequest.DateOfBirth))
                    {
                        IWebElement birthDateInput = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDOB")));
                        birthDateInput.SendKeys(createClaimsRequest.DateOfBirth);

                        IWebElement doneButton = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                        doneButton.Click();
                        Console.WriteLine("[INFO] Birthdate added.");
                    }

                    if (!string.IsNullOrEmpty(createClaimsRequest.Gender))
                    {
                        IWebElement genderDropdown = WaitUntil(driver, SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ddGender")));
                        SelectElement selectElement = new SelectElement(genderDropdown);
                        selectElement.SelectByText(createClaimsRequest.Gender);
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
}

