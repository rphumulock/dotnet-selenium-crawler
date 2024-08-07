using AngleSharp.Io;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

public class ServiceRequest
{
    public string ID { get; set; }
    public string SRID { get; set; }
    public string SRAuth { get; set; }
    public string AuthApprov { get; set; }
    public string AuthStatus { get; set; }
    public string ProvSite { get; set; }
    public string Phone { get; set; }
    public string Procedure { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Units { get; set; }
    public string SubmissionDate { get; set; }
    public string ModifiedDate { get; set; }
}

namespace HAISelenium
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = null;

            try
            {
                LogCurrentUserInfo();

                new DriverManager().SetUpDriver(new ChromeConfig());
                var options = SetupChromeOptions();

                driver = new ChromeDriver(options);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

                PerformActions(driver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                //driver?.Close();
                //driver?.Quit();
                Console.WriteLine("Browser closed.");
            }
        }

        private static void LogCurrentUserInfo()
        {
            string userName = Environment.UserName;
            string userDomainName = Environment.UserDomainName;
            Console.WriteLine($"Current User: {userDomainName}\\{userName}");
        }

        private static string GetEnvironmentVariableOrThrow(string variable)
        {
            string? value = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{variable} is not set in the environment variables.");
            }
            return value;
        }

        private static void VerifyDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }
        }

        private static ChromeOptions SetupChromeOptions()
        {
            string userDataDir = GetEnvironmentVariableOrThrow("CHROME_USER_DATA_DIR");
            string profileDir = GetEnvironmentVariableOrThrow("CHROME_PROFILE_DIR");

            VerifyDirectoryExists(userDataDir);
            VerifyDirectoryExists(Path.Combine(userDataDir, profileDir));

            var options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={userDataDir}");
            options.AddArgument($"--profile-directory={profileDir}");
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--enable-features=NewUsbBackend");

            Console.WriteLine("Chrome options set.");
            return options;
        }

        private static void PerformActions(IWebDriver driver)
        {
            string url = GetEnvironmentVariableOrThrow("URL");
            string username = GetEnvironmentVariableOrThrow("USERNAME");
            string password = GetEnvironmentVariableOrThrow("PASSWORD");

            Retry(() => NavigateToSite(driver, url), 3, "Failed to navigate to site. Retrying...");
            Retry(() => PerformLogin(driver, username, password), 3, "Login failed. Retrying...");
            Retry(() => NavigateToMembershipSearch(driver), 3, "Failed to navigate to Membership Search. Retrying...");
            Retry(() => LookupPatient(driver), 3, "Failed to look up patient. Retrying...");
            Retry(() => SelectPatient(driver), 3, "Failed to select patient. Retrying...");
            Retry(() => NavigateTAuthorizationRequests(driver), 3, "Failed to navigate to Authorization Requests. Retrying...");
            Retry(() => SelectClaim(driver), 3, "Failed to get Claim. Retrying...");
            //Retry(() => TestRun(driver), 3, "Failed test run");
        }

        private static void NavigateToSite(IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);

            Console.WriteLine($"Navigated to {url}");
            Console.WriteLine($"Title: {driver.Title}");
            Console.WriteLine($"URL: {driver.Url}");
        }

        private static void PerformLogin(IWebDriver driver, string username, string password)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement usernameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Username")));
            IWebElement passwordInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Password")));
            IWebElement submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("SubmitButton")));

            usernameInput.Clear();
            usernameInput.SendKeys(username);

            passwordInput.Clear();
            passwordInput.SendKeys(password);

            submitButton.Click();

            Console.WriteLine("Login form submitted.");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".fixed-top.header-text")));
        }

        private static void NavigateToMembershipSearch(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement membershipDropdownToggle = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-udfname='Membership']")));
            membershipDropdownToggle.Click();
            Console.WriteLine("Membership dropdown clicked.");

            IWebElement searchLink = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[data-udfname='Search']")));
            searchLink.Click();
            Console.WriteLine("Search link clicked.");
        }

        private static void NavigateTAuthorizationRequests(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement membershipDropdownToggle = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-udfname='Authorization']")));
            membershipDropdownToggle.Click();
            Console.WriteLine("Membership dropdown clicked.");

            IWebElement searchLink = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[data-udfname='Requests']")));
            searchLink.Click();
            Console.WriteLine("Search link clicked.");
        }

        private static void LookupPatient(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            string pFirstName = GetEnvironmentVariableOrThrow("PATIENT_FIRST_NAME");
            string pLastName = GetEnvironmentVariableOrThrow("PATIENT_LAST_NAME");
            string pBirthday = GetEnvironmentVariableOrThrow("PATIENT_BIRTH_DAY");
            string pGender = GetEnvironmentVariableOrThrow("PATIENT_GENDER");

            if (!string.IsNullOrEmpty(pFirstName))
            {
                IWebElement firstNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtFirst")));
                firstNameInput.SendKeys(pFirstName);
                Console.WriteLine("First Name added.");
            }

            if (!string.IsNullOrEmpty(pLastName))
            {
                IWebElement lastNameInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtLast")));
                lastNameInput.SendKeys(pLastName);
                Console.WriteLine("Last Name added.");
            }

            if (!string.IsNullOrEmpty(pBirthday))
            {
                IWebElement birthDateInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("txtDOB")));
                birthDateInput.SendKeys(pBirthday);

                IWebElement doneButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button.ui-datepicker-close[data-handler='hide'][data-event='click']")));
                doneButton.Click();
                Console.WriteLine("Birthday added.");
            }

            if (!string.IsNullOrEmpty(pGender))
            {
                IWebElement genderDropdown = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ddGender")));
                SelectElement selectElement = new SelectElement(genderDropdown);
                selectElement.SelectByText(pGender);
                Console.WriteLine("Gender selected.");
            }

            IWebElement searchButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button#tran1.btn.btn-info.btn-sm.rounded")));
            searchButton.Click();
            Console.WriteLine("Search button clicked successfully.");
        }

        private static void SelectPatient(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement patientGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("patientGrid")));
            patientGrid.Click();
            Console.WriteLine("Patient selected successfully.");
        }

        private static void TestRun(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement claimsGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("servicesGrid")));
            wait.Until(driver =>
            {
                var cells = claimsGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });

            // Date pattern to search for
            string month = "5";
            string year = "2024";
            // XPath to find elements that contain the partial date
            string xpath = $"//*[contains(text(), '{month}/') and contains(text(), '/{year}')]";

            try
            {
                IReadOnlyCollection<IWebElement> elements = driver.FindElements(By.XPath(xpath));

                IWebElement maxElement = null;
                int maxDay = -1;
                Regex dayRegex = new Regex($"{month}/(\\d+)/{year}");

                foreach (var element in elements)
                {
                    var match = dayRegex.Match(element.Text);
                    if (match.Success)
                    {
                        int day = int.Parse(match.Groups[1].Value);
                        if (day > maxDay)
                        {
                            maxDay = day;
                            maxElement = element;
                        }
                    }
                }

                if (maxElement != null)
                {
                    Console.WriteLine("Element with the greatest day value: " + maxElement.Text);
                    maxElement.Click();
                }
                else
                {
                    Console.WriteLine("No matching elements found with the specified date pattern.");
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No elements found with the partial date.");
            }
        }

        private static void SelectClaim(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            IWebElement servicesGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("servicesGrid")));
            wait.Until(driver =>
            {
                var cells = servicesGrid.FindElements(By.CssSelector("tbody td > :first-child"));
                return cells.Count > 0;
            });

            var trElements = servicesGrid.FindElements(By.CssSelector("tbody tr:not(:first-child)"));

            // Initialize dictionary to store row data
            List<List<string>> rowData = new List<List<string>>();
            for (int i = 0; i < trElements.Count; i++)
            {
                var tr = trElements[i];
                List<string> cellTexts = new List<string>();
                var tdElements = tr.FindElements(By.CssSelector("td:not([style*='display: none'])"));
                foreach (var td in tdElements)
                {
                    IWebElement firstChild = null;
                    try
                    {
                        firstChild = td.FindElement(By.CssSelector(":first-child"));
                    }
                    catch (NoSuchElementException)
                    {
                        continue;
                    }

                    if (firstChild != null)
                    {
                        cellTexts.Add(firstChild.Text.Trim());
                    }
                }
                rowData.Add(cellTexts);
            }

            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();
            foreach (var row in rowData)
            {
                ServiceRequest serviceRequest = new ServiceRequest
                {
                    ID = row[1],
                    SRID = row[2],
                    SRAuth = row[3],
                    AuthApprov = row[4],
                    AuthStatus = row[5],
                    ProvSite = row[6],
                    Phone = row[10],
                    Procedure = row[11],
                    StartDate = row[12],
                    EndDate = row[13],
                    Units = row[14],
                    SubmissionDate = row[21],
                    ModifiedDate = row[25]
                };

                serviceRequests.Add(serviceRequest);
            }

            // Example: Print the results in a more readable JSON-like format
            foreach (var request in serviceRequests)
            {
                Console.WriteLine("{");
                Console.WriteLine($"  \"ID\": \"{request.ID}\",");
                Console.WriteLine($"  \"SRID\": \"{request.SRID}\",");
                Console.WriteLine($"  \"SRAuth\": \"{request.SRAuth}\",");
                Console.WriteLine($"  \"AuthApprov\": \"{request.AuthApprov}\",");
                Console.WriteLine($"  \"AuthStatus\": \"{request.AuthStatus}\",");
                Console.WriteLine($"  \"ProvSite\": \"{request.ProvSite}\",");
                Console.WriteLine($"  \"Phone\": \"{request.Phone}\",");
                Console.WriteLine($"  \"Procedure\": \"{request.Procedure}\",");
                Console.WriteLine($"  \"StartDate\": \"{request.StartDate}\",");
                Console.WriteLine($"  \"EndDate\": \"{request.EndDate}\",");
                Console.WriteLine($"  \"Units\": \"{request.Units}\",");
                Console.WriteLine($"  \"SubmissionDate\": \"{request.SubmissionDate}\",");
                Console.WriteLine($"  \"ModifiedDate\": \"{request.ModifiedDate}\"");
                Console.WriteLine("}");
                Console.WriteLine(); // Add an extra line between entries for readability
            }
        }

        //private static GetClaimsHeaders()
        //{
        //    //IWebElement headerGrid = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("gview_servicesGrid")));
        //    //var headers = headerGrid.FindElements(By.CssSelector("thead th"));

        //    //List<string> headerTexts = new List<string>();
        //    //foreach (var header in headers)
        //    //{
        //    //    if (!string.IsNullOrEmpty(header.Text.Trim()))
        //    //    {
        //    //        headerTexts.Add(header.Text.Trim());
        //    //    }
        //    //}
        //    //foreach (var text in headerTexts)
        //    //{
        //    //    Console.WriteLine(text);
        //    //}

        //}

        private static void Retry(Action action, int retries, string errorMessage)
        {
            int attempt = 0;
            while (attempt < retries)
            {
                try
                {
                    action();
                    return;
                }
                catch (NoSuchElementException ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: Element not found. {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (ElementClickInterceptedException ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: Element click intercepted. {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (WebDriverTimeoutException ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: Timeout. {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"{errorMessage} Attempt {attempt} of {retries}. Error: {ex.Message}");
                    if (attempt >= retries) throw;
                }
            }
        }
    }
}
