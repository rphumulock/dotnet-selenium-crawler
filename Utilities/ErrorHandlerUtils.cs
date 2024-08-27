using OpenQA.Selenium;
using System.Net.Sockets;

namespace HAI_Selenium.Utilities
{
    internal static class ErrorHandlerUtils
    {
        //public static void AnalyzeAndHandleFinalException(Exception ex)
        //{
        //    if (IsNetworkError(ex))
        //    {
        //        HandleNetworkError(ex);
        //    }
        //    else if (IsSeleniumError(ex))
        //    {
        //        HandleSeleniumError(ex);
        //    }
        //    else
        //    {
        //        HandleUnexpectedError(ex);
        //    }
        //}

        public static void AnalyzeAndHandleFinalException(Exception ex)
        {
            if (ex is RecoverableError)
            {
                HandleRecoverableError(ex);
            }
            else if (ex is NonRecoverableError)
            {
                HandleNonRecoverableError(ex);
            }
        }

        private static bool IsNetworkError(Exception ex)
        {
            return ex is TimeoutException ||
                   (ex is WebDriverException webDriverEx &&
                   (webDriverEx.InnerException is SocketException ||
                    webDriverEx.InnerException is System.Net.WebException ||
                    webDriverEx.InnerException is System.Net.Http.HttpRequestException));
        }

        private static bool IsSeleniumError(Exception ex)
        {
            return ex is NoSuchElementException ||
                   ex is InvalidSelectorException ||
                   ex is StaleElementReferenceException ||
                   ex is ElementNotInteractableException ||
                   ex is ElementClickInterceptedException ||
                   ex is UnhandledAlertException ||
                   ex is WebDriverException;
        }
        private static void HandleRecoverableError(Exception ex)
        {
            Console.WriteLine($"[ERROR] Recoverable error: {ex.Message}");
            // Additional handling for network errors, like retrying or notifying the user
        }

        private static void HandleNonRecoverableError(Exception ex)
        {
            Console.WriteLine($"[ERROR] Non-Recoverable error: {ex.Message}");
            // Additional handling for network errors, like retrying or notifying the user
        }

        private static void HandleNetworkError(Exception ex)
        {
            Console.WriteLine($"[ERROR] Network-related error: {ex.Message}");
            // Additional handling for network errors, like retrying or notifying the user
        }

        private static void HandleSeleniumError(Exception ex)
        {
            if (ex is NoSuchElementException)
            {
                Console.WriteLine($"[ERROR] NoSuchElementException: {ex.Message}");
                // Additional handling for NoSuchElementException if needed
            }
            else if (ex is InvalidSelectorException)
            {
                Console.WriteLine($"[ERROR] InvalidSelectorException: {ex.Message}");
                // Additional handling for InvalidSelectorException if needed
            }
            else if (ex is StaleElementReferenceException)
            {
                Console.WriteLine($"[ERROR] StaleElementReferenceException: {ex.Message}");
                // Additional handling for StaleElementReferenceException if needed
            }
            else if (ex is ElementNotInteractableException)
            {
                Console.WriteLine($"[ERROR] ElementNotInteractableException: {ex.Message}");
                // Additional handling for ElementNotInteractableException if needed
            }
            else if (ex is ElementClickInterceptedException)
            {
                Console.WriteLine($"[ERROR] ElementClickInterceptedException: {ex.Message}");
                // Additional handling for ElementClickInterceptedException if needed
            }
            else if (ex is UnhandledAlertException)
            {
                Console.WriteLine($"[ERROR] UnhandledAlertException: {ex.Message}");
                // Additional handling for UnhandledAlertException if needed
            }
            else if (ex is WebDriverException)
            {
                Console.WriteLine($"[ERROR] WebDriverException: {ex.Message}");
                // Additional handling for generic WebDriverException if needed
            }
            else
            {
                Console.WriteLine($"[ERROR] Unknown Selenium error: {ex.Message}");
            }
        }

        private static void HandleUnexpectedError(Exception ex)
        {
            Console.WriteLine($"[ERROR] Unexpected exception: {ex.Message}");
            Console.WriteLine($"[STACK TRACE] {ex.StackTrace}");
            // Additional handling for unexpected exceptions
        }
    }
}