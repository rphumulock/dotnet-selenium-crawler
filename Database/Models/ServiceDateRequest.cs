namespace HAI_Selenium.Database.Models
{
    public class ServiceDateRequest
    {
        public int Id { get; set; }
        public string ServiceDate { get; set; }
        public string Counselor { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Other { get; set; }
        public string TreatmentType { get; set; }
        public int InvoiceRequestId { get; set; }
        public InvoiceRequest InvoiceRequest { get; set; }
    }
}
