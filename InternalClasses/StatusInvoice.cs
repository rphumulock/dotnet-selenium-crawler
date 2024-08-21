namespace HAI_Selenium.InternalClasses
{
    internal class StatusInvoice
    {
        public required string InvoiceID { get; set; }
        public required List<Claim> Claims { get; set; }
       
    }
}