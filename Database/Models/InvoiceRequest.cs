namespace HAI_Selenium.Database.Models
{
    public class InvoiceRequest
    {
        public int Id { get; set; } // Serial primary key
        public string RequestorName { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
