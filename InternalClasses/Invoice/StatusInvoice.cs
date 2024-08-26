using HAI_Selenium.InternalClasses.Status;

namespace HAI_Selenium.InternalClasses.Invoice
{
    internal class StatusInvoice
    {
        public required string InvoiceID { get; set; }
        public required List<ClaimRequest> ClaimRequests { get; set; }
    }
}