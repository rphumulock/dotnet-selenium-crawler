using OpenQA.Selenium;
using System.Net.Sockets;
using HAI_Selenium.Data;
using HAI_Selenium.Database.Models;
using Serilog;
using System;
using HAI_Selenium.Workflow.Steps.CreateRequest;
using HAI_Selenium.Exceptions;
using System.Linq;
using HAI_Selenium.InternalClasses.CreateRequest;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace HAI_Selenium.Utilities
{
    // Correct single definition of HAI_Error class
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

    // Define ErrorType Enum
    internal enum ErrorType
    {
        Recoverable,
        NonRecoverable
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
            var errorType = ErrorType.Recoverable; // Default to recoverable
            var errorMessage = ex switch
            {
                NoSuchElementException => "NoSuchElementException",
                InvalidSelectorException => "InvalidSelectorException",
                StaleElementReferenceException => "StaleElementReferenceException",
                ElementNotInteractableException => "ElementNotInteractableException",
                //ElementClickInterceptedException => "ElementClickInterceptedException",
                UnhandledAlertException => "UnhandledAlertException",
                _ => "Unknown Selenium error"
            };

            if (ex is ElementClickInterceptedException || ex is UnhandledAlertException)
            {
                errorType = ErrorType.NonRecoverable; // Mark specific errors as non-recoverable
            }

            Log.Error(ex, $"{errorMessage}: {{Message}}", ex.Message);
            EnterErrorIntoDatabase(new HAI_Error(errorType, ex));
        }

        private static void HandleUnexpectedError(Exception ex)
        {
            Log.Error(ex, "Unexpected error: {Message}", ex.Message);

            if (ex is HAIException haiException)
            {
                EnterErrorIntoDatabase(new HAI_Error(ErrorType.Recoverable, haiException));
            }
            else
            {
                throw ex; // Properly rethrow the original exception
            }
        }

        private static void EnterErrorIntoDatabase(HAI_Error error)
        {
            try
            {
                Log.Information("Executing workflow step for error handling");

                using (var dbContext = new ApplicationDbContext())
                {
                    if (error.Exception is HAIException haiException)
                    {
                        var contextData = haiException.Context;
                        InvoiceRequest createClaimsRequest = contextData.Get<InvoiceRequest>("InvoiceRequest");

                        // Retrieve current batch and remaining batches from context
                        ICollection<ServiceDateRequest> currentBatch = contextData.Get<ICollection<ServiceDateRequest>>("CurrentBatchServiceDateRequests");
                        ICollection<ICollection<ServiceDateRequest>> remainingBatches = contextData.Get<ICollection<ICollection<ServiceDateRequest>>>("RemainingBatchesServiceDateRequests");
                        ICollection<ServiceDateRequest> allServiceDateRequests = new List<ServiceDateRequest>();
                        allServiceDateRequests = currentBatch.ToList();
                        foreach (var batch in remainingBatches)
                        {
                            allServiceDateRequests = allServiceDateRequests.Concat(batch).ToList();
                        }

                        // Create a new InvoiceRequest with the flattened list
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

                        dbContext.InvoiceRequests.Add(newInvoiceRequest);
                        dbContext.SaveChanges();
                    }
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
