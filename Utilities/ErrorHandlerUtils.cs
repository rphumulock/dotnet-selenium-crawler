using OpenQA.Selenium;
using System.Net.Sockets;
using HAI_Selenium.Data;
using HAI_Selenium.Database.Models;
using Serilog;
using System;

namespace HAI_Selenium.Utilities
{
    internal enum ErrorType
    {
        Recoverable,
        NonRecoverable
    }

    internal class HAI_Error
    {
        public ErrorType Type { get; }
        public Exception Exception { get; }

        public HAI_Error(ErrorType type, Exception exception)
        {
            Type = type;
            Exception = exception;
        }

        public override string ToString()
        {
            return $"RecoverableError: {Exception.Message} {(Exception.InnerException != null ? "-> " + Exception.InnerException.ToString() : string.Empty)}";
        }
    }

    internal static class ErrorHandlerUtils
    {
        public static void AnalyzeAndHandleFinalException(Exception ex)
        {
            if (IsNetworkError(ex))
            {
                HandleNetworkError(ex);
            }
            else if (IsSeleniumError(ex))
            {
                HandleSeleniumError(ex);
            }
            else
            {
                HandleUnexpectedError(ex);
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

        private static void HandleNetworkError(Exception ex)
        {
            Log.Error(ex, "Network-related error: {Message}", ex.Message);
            EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
        }

        private static void HandleSeleniumError(Exception ex)
        {
            if (ex is NoSuchElementException)
            {
                Log.Error(ex, "NoSuchElementException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
            }
            else if (ex is InvalidSelectorException)
            {
                Log.Error(ex, "InvalidSelectorException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
            }
            else if (ex is StaleElementReferenceException)
            {
                Log.Error(ex, "StaleElementReferenceException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
            }
            else if (ex is ElementNotInteractableException)
            {
                Log.Error(ex, "ElementNotInteractableException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
            }
            else if (ex is ElementClickInterceptedException)
            {
                Log.Error(ex, "ElementClickInterceptedException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.NonRecoverable, ex));
            }
            else if (ex is UnhandledAlertException)
            {
                Log.Error(ex, "UnhandledAlertException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.NonRecoverable, ex));
            }
            else if (ex is WebDriverException)
            {
                Log.Error(ex, "WebDriverException: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
            }
            else
            {
                Log.Error(ex, "Unknown Selenium error: {Message}", ex.Message);
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
            }
        }

        private static void HandleUnexpectedError(Exception ex)
        {
            Log.Error(ex, "Unexpected error: {Message}", ex.Message);
            EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex));
        }

        private static void EnterErrorIntoDatabase(HAI_Error error)
        {
            try
            {
                // Initialize the database context
                using (var dbContext = new ApplicationDbContext())
                {
                    // Example: Add a new InvoiceRequest
                    var newInvoiceRequest = new InvoiceRequest
                    {
                        // Set properties for your InvoiceRequest object
                    };
                    dbContext.InvoiceRequests.Add(newInvoiceRequest);
                    dbContext.SaveChanges(); // Save changes to the database

                    // Example: Query the InvoiceRequests table
                    var invoiceRequests = dbContext.InvoiceRequests.ToList();
                    foreach (var request in invoiceRequests)
                    {
                        Log.Information("InvoiceRequest ID: {Id}", request.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error writing to Database: {Message}", ex.Message);
                throw;
            }
        }
    }
}
