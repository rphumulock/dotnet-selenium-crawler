using HAI_Selenium.InternalClasses.Status;

namespace HAI_Selenium.InternalClasses.Invoice
{
    internal class StatusInvoice
    {
        public required string InvoiceID { get; set; }
        public required List<Claim> Claims { get; set; }

    }
}