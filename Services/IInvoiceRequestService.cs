using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Services
{
    public interface IInvoiceRequestService
    {

        Task<List<ServiceDateRequest>> GetServiceDateRequestsByInvoiceIdAsync(int invoiceRequestId);

        Task DeleteServiceDateRequestsByIdsAsync(IEnumerable<int> serviceDateRequestIds);

        Task DeleteServiceDateRequestsByInvoiceIdAsync(int invoiceRequestId);

        Task SaveServiceDateRequestsAsync(IEnumerable<ServiceDateRequest> serviceDateRequests);
    }
}
