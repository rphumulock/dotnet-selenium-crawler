using OpenQA.Selenium;
using System.Net.Sockets;
using HAI_Selenium.Database.Models;
using Serilog;
using HAI_Selenium.Exceptions;
using HAI_Selenium.Services;
using HAI_Selenium.InternalClasses.CreateRequest;

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
        public static async Task AnalyzeAndHandleFinalExceptionAsync(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            if (IsNetworkError(ex))
            {
                await HandleNetworkErrorAsync(ex, invoiceRequestService);
            }
            else if (IsSeleniumError(ex))
            {
                await HandleSeleniumErrorAsync(ex, invoiceRequestService);
            }
            else
            {
                await HandleUnexpectedErrorAsync(ex, invoiceRequestService);
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

        private static async Task HandleNetworkErrorAsync(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            Log.Error(ex, "Network-related error: {Message}", ex.Message);
            await EnterErrorIntoDatabaseAsync(new HAI_Error(ErrorType.Recoverable, ex), invoiceRequestService);
        }

        private static async Task HandleSeleniumErrorAsync(Exception ex, IInvoiceRequestService invoiceRequestService)
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
            await EnterErrorIntoDatabaseAsync(new HAI_Error(errorType, ex), invoiceRequestService);
        }

        private static async Task HandleUnexpectedErrorAsync(Exception ex, IInvoiceRequestService invoiceRequestService)
        {
            Log.Error(ex, "Unexpected error: {Message}", ex.Message);

            if (ex is HAIException haiException)
            {
                await EnterErrorIntoDatabaseAsync(new HAI_Error(ErrorType.Recoverable, haiException), invoiceRequestService);
            }
            else
            {
                Log.Error(ex, "Unexpected error: {Message}", ex.Message);
            }
        }

        private static async Task EnterErrorIntoDatabaseAsync(HAI_Error error, IInvoiceRequestService invoiceRequestService)
        {
            try
            {
                Log.Information("Executing workflow step for error handling");

                if (error.Exception is HAIException haiException)
                {
                    var contextData = haiException.Context;
                    var mockRequest = contextData.Get<InvoiceRequest>("MockRequest");

                    var currentBatch = contextData.Get<ICollection<ServiceDateRequest>>("CurrentBatchServiceDateRequests");
                    var remainingBatches = contextData.Get<ICollection<ICollection<ServiceDateRequest>>>("RemainingBatchesServiceDateRequests");

                    // Combine all batches into one list
                    var allServiceDateRequests = currentBatch.ToList();
                    foreach (var batch in remainingBatches)
                    {
                        allServiceDateRequests.AddRange(batch);
                    }

                    // Fetch existing ServiceDateRequests from the database
                    var existingServiceDateRequests = await invoiceRequestService.GetServiceDateRequestsByInvoiceIdAsync(int.Parse(mockRequest.InvoiceId));

                    // Determine which ServiceDateRequests need to be deleted
                    var serviceDateRequestsToDelete = existingServiceDateRequests
                        .Where(existing => !allServiceDateRequests.Any(newRequest =>
                            DateTime.Parse(newRequest.ServiceDate) == DateTime.Parse(existing.ServiceDate)))
                        .ToList();

                    // Delete the ServiceDateRequests that do not exist in allServiceDateRequests
                    if (serviceDateRequestsToDelete.Any())
                    {
                        await invoiceRequestService.DeleteServiceDateRequestsByIdsAsync(serviceDateRequestsToDelete.Select(sdr => sdr.Id));
                    }

                    // Prepare to save the ServiceDateRequests that exist in allServiceDateRequests
                    await invoiceRequestService.SaveServiceDateRequestsAsync(allServiceDateRequests);
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
