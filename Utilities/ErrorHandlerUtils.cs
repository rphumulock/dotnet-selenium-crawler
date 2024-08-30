using OpenQA.Selenium;
using System.Net.Sockets;
using HAI_Selenium.Database.Models;
using Serilog;
using HAI_Selenium.Exceptions;
using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Services;

namespace HAI_Selenium.Utilities
{
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
            return $"RecoverableError: {Exception.Message} {(Exception.InnerException != null ? "-> " + Exception.InnerException : string.Empty)}";
        }
    }

    internal enum ErrorType
    {
        Recoverable,
        NonRecoverable
    }

    internal static class ErrorHandlerUtils
    {
        public static void AnalyzeAndHandleFinalException(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            if (IsNetworkError(ex))
            {
                HandleNetworkError(ex, invoiceRequestService);
            }
            else if (IsSeleniumError(ex))
            {
                HandleSeleniumError(ex, invoiceRequestService);
            }
            else
            {
                HandleUnexpectedError(ex, invoiceRequestService);
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

        private static void HandleNetworkError(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            Log.Error(ex, "Network-related error: {Message}", ex.Message);
            EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, ex), invoiceRequestService);
        }

        private static void HandleSeleniumError(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            var errorType = ex is ElementClickInterceptedException || ex is UnhandledAlertException
                ? ErrorType.NonRecoverable
                : ErrorType.Recoverable;

            var errorMessage = ex switch
            {
                NoSuchElementException => "NoSuchElementException",
                InvalidSelectorException => "InvalidSelectorException",
                StaleElementReferenceException => "StaleElementReferenceException",
                ElementNotInteractableException => "ElementNotInteractableException",
                _ => "Unknown Selenium error"
            };

            Log.Error(ex, $"{errorMessage}: {{Message}}", ex.Message);
            EnterErrorIntoDatabase(new HAI_Error(errorType, ex), invoiceRequestService);
        }

        private static void HandleUnexpectedError(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            Log.Error(ex, "Unexpected error: {Message}", ex.Message);

            if (ex is HAIException haiException)
            {
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, haiException), invoiceRequestService);
            }
            else
            {
                Log.Error(ex, "Unexpected erro FIX MEEEE IN ERROR HANDLER: {Message}", ex.Message);
            }
        }

        private static void EnterErrorIntoDatabase(HAI_Error error, IInvoiceRequestService invoiceRequestService)
        {
            try
            {
                Log.Information("Executing workflow step for error handling");

                if (error.Exception is HAIException haiException)
                {
                    var contextData = haiException.Context;
                    InvoiceRequest createClaimsRequest = contextData.Get<InvoiceRequest>("InvoiceRequest");

                    var currentBatch = contextData.Get<ICollection<ServiceDateRequest>>("CurrentBatchServiceDateRequests");
                    var remainingBatches = contextData.Get<ICollection<ICollection<ServiceDateRequest>>>("RemainingBatchesServiceDateRequests");

                    var allServiceDateRequests = currentBatch.ToList();
                    foreach (var batch in remainingBatches)
                    {
                        allServiceDateRequests = allServiceDateRequests.Concat(batch).ToList();
                    }

                    var newInvoiceRequest = new InvoiceRequest
                    {
                        Id = createClaimsRequest.Id,
                        InvoiceId = createClaimsRequest.InvoiceId,
                        FirstName = createClaimsRequest.FirstName,
                        LastName = createClaimsRequest.LastName,
                        PolicyNumber = createClaimsRequest.PolicyNumber,
                        DiagnosisCodes = createClaimsRequest.DiagnosisCodes,
                        DateOfBirth = createClaimsRequest.DateOfBirth,
                        Gender = createClaimsRequest.Gender,
                        ServiceDateRequests = allServiceDateRequests
                    };

                    // Use the service to handle database operations
                    invoiceRequestService.AddInvoiceRequest(newInvoiceRequest);
                }
            }
            catch (Exception dbEx)
            {
                Log.Error(dbEx, "Error writing to Database: {Message}", dbEx.Message);
                throw;
            }
        }
    }
}
