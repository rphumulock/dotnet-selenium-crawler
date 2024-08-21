using dotenv.net;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using HAI_Selenium.InternalClasses;
using HAISelenium.InternalClasses;

namespace HAI_Selenium.Utils
{
    internal static class Utilities
    {
        internal static IWebDriver SetupDriver()
        {
            Console.WriteLine("[ACTION] Setting up WebDriver...");

            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = SetupChromeOptions();
            var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

            Console.WriteLine("[SUCCESS] WebDriver setup complete.");

            return driver;
        }

        internal static void LoadEnvVariables()
        {
            Console.WriteLine("[ACTION] Loading environment variables...");
            DotEnv.Load();
            Console.WriteLine("[SUCCESS] Environment variables loaded.");
        }

        internal static T LoadJsonFile<T>(string filePath)
        {
            Console.WriteLine("[ACTION] Loading JSON file...");

            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<T>(json);
                Console.WriteLine("[SUCCESS] JSON file loaded and parsed.");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load JSON file: {ex.Message}");
                throw;
            }
        }

        internal static ChromeOptions SetupChromeOptions()
        {
            var userDataDir = GetEnvironmentVariableOrThrow("CHROME_USER_DATA_DIR");
            var profileDir = GetEnvironmentVariableOrThrow("CHROME_PROFILE_DIR");

            VerifyDirectoryExists(userDataDir);
            VerifyDirectoryExists(Path.Combine(userDataDir, profileDir));

            var options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={userDataDir}");
            options.AddArgument($"--profile-directory={profileDir}");
            options.AddArgument("--remote-debugging-port=9222");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--enable-features=NewUsbBackend");

            Console.WriteLine("[INFO] Chrome options configured.");
            return options;
        }

        internal static void VerifyDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"[ERROR] Directory not found: {path}");
            }
        }

        internal static string GetEnvironmentVariableOrThrow(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"[ERROR] Environment variable '{key}' is not set.");
            }
            return value;
        }

        internal static void LogCurrentUserInfo()
        {
            Console.WriteLine("[ACTION] Logging current user info...");

            var userName = GetEnvironmentVariableOrThrow("USERNAME");
            var userDomainName = Environment.UserDomainName;
            Console.WriteLine($"[INFO] Current User: {userDomainName}\\{userName}");

            Console.WriteLine("[SUCCESS] User info logged.");
        }

        internal static void Retry(Action action, int retries, string errorMessage)
        {
            int attempt = 0;
            while (attempt < retries)
            {
                try
                {
                    action();
                    return;
                }
                catch (WebDriverException ex) when (ex is NoSuchElementException or ElementClickInterceptedException or WebDriverTimeoutException)
                {
                    attempt++;
                    Console.WriteLine($"[WARN] {errorMessage} Attempt {attempt} of {retries}. Error: {ex.Message}");
                    if (attempt >= retries) throw;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine($"[ERROR] {errorMessage} Attempt {attempt} of {retries}. Error: {ex.Message}");
                    if (attempt >= retries) throw;
                }
            }
        }

        internal static FormDataForProcessing CreateFormDataForProcessing(Invoice invoice, PaymentData paymentData, ServiceRequest authNumberServiceRequest)
        {
            var serviceDateRequests = invoice.ServiceDateRequests;
            var serviceDateFormDataList = serviceDateRequests.Select(serviceDateRequest => new ServiceDateFormData
            {
                StartDate = serviceDateRequest.ServiceDate,
                PlaceOfService = "15",
                CPT = "H2016",
                DiagnosisPointer = GetDiagnosisPointer(invoice.DiagnosisCodes.Count()),
                ChargesDollars = "1.",
                ChargesCents = "00",
                Units = "1"
            }).ToList();

            var latestServiceDate = FindLatestServiceDate(serviceDateRequests);
            var payDate = CalculatePayDate(authNumberServiceRequest, latestServiceDate);
            var payment = paymentData.GetAmount(serviceDateRequests[0].TreatmentType, serviceDateFormDataList.Count);
            var paymentDollars = payment.Split('.')[0] + ".";
            var paymentCents = payment.Split('.')[1];
            serviceDateFormDataList.Add(new ServiceDateFormData
            {
                StartDate = payDate,
                PlaceOfService = "15",
                CPT = "H2018",
                DiagnosisPointer = GetDiagnosisPointer(invoice.DiagnosisCodes.Count()),
                ChargesDollars = paymentDollars,
                ChargesCents = paymentCents,
                Units = "1"
            });

            var batchedServiceDateFormData = BatchServiceDateFormData(serviceDateFormDataList);

            return new FormDataForProcessing
            {
                patientFormData = new PatientFormData
                {
                    patientDiagnosisCodes = invoice.DiagnosisCodes.Select(code => code.Replace(".", "")).ToList(),
                    patientPolicyNumber = invoice.PolicyNumber,
                    authNumber = authNumberServiceRequest.SRAuth,
                },
                serviceDatesFormData = batchedServiceDateFormData
            };
        }

        private static string GetDiagnosisPointer(int diagnosisCodeCount)
        {
            return new string(Enumerable.Range('A', diagnosisCodeCount).Select(x => (char)x).ToArray());
        }

        private static List<List<ServiceDateFormData>> BatchServiceDateFormData(List<ServiceDateFormData> serviceDateFormDataList)
        {
            const int batchSize = 6;
            var batchedServiceDateFormData = new List<List<ServiceDateFormData>>();

            for (int i = 0; i < serviceDateFormDataList.Count; i += batchSize)
            {
                batchedServiceDateFormData.Add(serviceDateFormDataList.GetRange(i, Math.Min(batchSize, serviceDateFormDataList.Count - i)));
            }

            return batchedServiceDateFormData;
        }

        internal static string CalculatePayDate(ServiceRequest authNumberServiceRequest, ServiceDateRequest serviceDateRequest)
        {
            Console.WriteLine("[ACTION] Calculating pay date...");

            var authNumberServiceRequestParts = authNumberServiceRequest.StartDate.Split('/');
            var serviceDateRequestParts = serviceDateRequest.ServiceDate.Split('/');

            if (!int.TryParse(authNumberServiceRequestParts[1], out int authNumberServiceRequestDay) ||
                !int.TryParse(serviceDateRequestParts[1], out int serviceDateRequestDay))
            {
                throw new ArgumentException("Invalid date format in service date or service request start date.");
            }

            string payDate;

            if (authNumberServiceRequestDay >= serviceDateRequestDay)
            {
                payDate = string.Join("/", authNumberServiceRequestParts);
            }
            else
            {
                string lastDayOfMonth = GetLastDayOfMonth(serviceDateRequestParts[0], serviceDateRequestParts[2]);
                int payDay = serviceDateRequestParts[1] == lastDayOfMonth ? serviceDateRequestDay : serviceDateRequestDay + 1;
                payDate = $"{serviceDateRequestParts[0]}/{payDay}/{serviceDateRequestParts[2]}";
            }

            return payDate;
        }
        internal static string ValidateServiceDateMonth(Invoice invoice)
        {
            Console.WriteLine("[ACTION] Validating service dates month ...");

            if (invoice.ServiceDateRequests == null || invoice.ServiceDateRequests.Count == 0)
            {
                throw new ArgumentNullException("[ERROR] ServiceDateRequests cannot be null or empty.");
            }

            string serviceDateMonth = null;

            foreach (var ServiceDateRequest in invoice.ServiceDateRequests)
            {
                string[] dateParts = ServiceDateRequest.ServiceDate.Split('/');
                string currentMonth = dateParts[0];

                if (serviceDateMonth == null)
                {
                    serviceDateMonth = currentMonth;
                }
                else if (currentMonth != serviceDateMonth)
                {
                    throw new InvalidOperationException($"[ERROR] Mismatch found: expected month {serviceDateMonth}, but found {currentMonth}.");
                }
            }

            Console.WriteLine($"[SUCCESS] Invoice data loaded and service month validated: {serviceDateMonth}.");

            return serviceDateMonth;
        }

        internal static ServiceDateRequest FindLatestServiceDate(List<ServiceDateRequest> serviceDateRequests)
        {
            string[] formats = { "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy" };

            return serviceDateRequests
                .Select(serviceDateRequest =>
                {
                    if (!DateTime.TryParseExact(serviceDateRequest.ServiceDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        throw new InvalidOperationException($"Invalid date format: {serviceDateRequest.ServiceDate}");
                    }
                    return new { serviceDateRequest, parsedDate };
                })
                .OrderByDescending(x => x.parsedDate)
                .First()
                .serviceDateRequest;
        }

        internal static WebDriverWait CreateWebDriverWait(IWebDriver driver, int timeoutSeconds = 20)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        }

        public static string GetLastDayOfMonth(string monthString, string yearString)
        {
            if (!int.TryParse(monthString, out int month) || month is < 1 or > 12)
            {
                throw new ArgumentException("Invalid month format. Please enter a valid month as a number (e.g., '5' or '05').");
            }

            if (!int.TryParse(yearString, out int year))
            {
                throw new ArgumentException("Invalid year format. Please enter a valid year as a number (e.g., '2023').");
            }

            return DateTime.DaysInMonth(year, month).ToString();
        }
    }
}
