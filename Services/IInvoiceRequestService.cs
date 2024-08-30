using HAI_Selenium.Database.Models;

namespace HAI_Selenium.Services
{
    public interface IInvoiceRequestService
    {
        Task DeleteInvoiceIfExistsAsync(int invoiceId);
        Task<InvoiceRequest> GetInvoiceRequestByIdAsync(int invoiceId);
        Task AddInvoiceRequest(InvoiceRequest invoiceRequest);
    }
}
