using HAI_Selenium.Database.Models;
using HAI_Selenium.Workflow.Classes;
using OpenQA.Selenium;
using Serilog;
using HAI_Selenium.Services;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Workflow.Steps
{
    public class SetupInvoiceData(WorkflowContext context, IInvoiceRequestService invoiceRequestService) : WorkflowStepBase(context)
    {
        protected override async Task PerformStepAsync(IWebDriver driver)
        {
            Log.Information("[ACTION] Setting up Invoice Data...");

            InvoiceRequest mockRequest = Context.Get<InvoiceRequest>("MockRequest");

            ICollection<ServiceDateRequest> failedServiceDates = await invoiceRequestService.GetServiceDateRequestsByInvoiceIdAsync(int.Parse(mockRequest.InvoiceId));

            if (failedServiceDates.Count > 0)
            {
                Log.Information("Item with ID {ItemId} exists in the database.", mockRequest.InvoiceId);
                Context.Set("ServiceDateRequests", failedServiceDates);
            }
            else
            {
                Log.Warning("Item with ID {ItemId} does not exist in the database.", mockRequest.InvoiceId);

                // Create a new ICollection<ServiceDateRequest> from mockRequest.ServiceDateRequests
                ICollection<ServiceDateRequest> serviceDateRequests = new List<ServiceDateRequest>();

                foreach (var sdr in mockRequest.ServiceDateRequests)
                {
                    serviceDateRequests.Add(new ServiceDateRequest
                    {
                        Id = sdr.Id,
                        InvoiceRequestId = int.Parse(mockRequest.InvoiceId), // Assuming InvoiceId is a string and needs parsing
                        ServiceDate = sdr.ServiceDate,
                        Counselor = sdr.Counselor,
                        StartTime = sdr.StartTime,
                        EndTime = sdr.EndTime,
                        Other = sdr.Other,
                        TreatmentType = sdr.TreatmentType
                    });
                }

                // Set the newly created ServiceDateRequests in the context
                Context.Set("ServiceDateRequests", serviceDateRequests);
            }
            Context.Set("ServiceDateRequestsCount", mockRequest.ServiceDateRequests.Count);
            Context.Set("TreatmentType", mockRequest.ServiceDateRequests.ElementAt(0).TreatmentType);

            Log.Information("[SUCCESS] Setup Invoice Data.");

            return;
        }
    }
}

