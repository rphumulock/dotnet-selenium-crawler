using Microsoft.EntityFrameworkCore;
using HAI_Selenium.Data;
using HAI_Selenium.Database.Models;

public class InvoiceService
{
    private readonly ApplicationDbContext _context;

    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceRequest> GetInvoiceRequestWithServiceDatesAsync(int invoiceRequestId)
    {
        var invoiceRequest = await _context.InvoiceRequests
            .Include(ir => ir.ServiceDateRequests) // Eager loading of related ServiceDateRequests
            .FirstOrDefaultAsync(ir => ir.Id == invoiceRequestId);

        return invoiceRequest;
    }
}
