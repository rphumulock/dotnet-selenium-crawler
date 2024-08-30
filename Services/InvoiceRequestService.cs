using HAI_Selenium.Data;
using HAI_Selenium.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HAI_Selenium.Services
{
    public class InvoiceRequestService(ApplicationDbContext dbContext) : IInvoiceRequestService
    {
        public async Task<List<ServiceDateRequest>> GetServiceDateRequestsByInvoiceIdAsync(int invoiceRequestId)
        {
            return await dbContext.ServiceDateRequests
                .Where(sdr => sdr.InvoiceRequestId == invoiceRequestId)
                .ToListAsync();
        }

        public async Task DeleteServiceDateRequestsByIdsAsync(IEnumerable<int> serviceDateRequestIds)
        {
            var serviceDateRequestsToDelete = await dbContext.ServiceDateRequests
                .Where(sdr => serviceDateRequestIds.Contains(sdr.Id))
                .ToListAsync();

            dbContext.ServiceDateRequests.RemoveRange(serviceDateRequestsToDelete);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteServiceDateRequestsByInvoiceIdAsync(int invoiceRequestId)
        {
            var serviceDateRequestsToDelete = await dbContext.ServiceDateRequests
                .Where(sdr => sdr.InvoiceRequestId == invoiceRequestId)
                .ToListAsync();

            dbContext.ServiceDateRequests.RemoveRange(serviceDateRequestsToDelete);
            await dbContext.SaveChangesAsync();
        }

        public async Task SaveServiceDateRequestsAsync(IEnumerable<ServiceDateRequest> serviceDateRequests)
        {
            foreach (var serviceDateRequest in serviceDateRequests)
            {
                if (serviceDateRequest.Id == 0)
                {
                    dbContext.ServiceDateRequests.Add(serviceDateRequest);
                }
                else
                {
                    dbContext.ServiceDateRequests.Update(serviceDateRequest);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
