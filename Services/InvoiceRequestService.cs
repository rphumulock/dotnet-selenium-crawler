using HAI_Selenium.Data;
using HAI_Selenium.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HAI_Selenium.Services
{
    public class InvoiceRequestService : IInvoiceRequestService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteInvoiceIfExistsAsync(int invoiceId)
        {
            var invoiceRequest = await _context.InvoiceRequests.FindAsync(invoiceId);
            if (invoiceRequest != null)
            {
                _context.InvoiceRequests.Remove(invoiceRequest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<InvoiceRequest> GetInvoiceRequestByIdAsync(int invoiceId)
        {
            return await _context.InvoiceRequests
                .Include(ir => ir.ServiceDateRequests)
                .SingleOrDefaultAsync(ir => ir.InvoiceId == invoiceId);
        }

        public async Task AddInvoiceRequest(InvoiceRequest invoiceRequest)
        {
            _context.InvoiceRequests.Add(invoiceRequest);
            await _context.SaveChangesAsync();
        }
    }
}
