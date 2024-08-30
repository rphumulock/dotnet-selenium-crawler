namespace HAI_Selenium.Database.Models
{

    public class ServiceDateRequest
    {

        public required int Id { get; set; }

        public required int InvoiceRequestId { get; set; }

        public required string ServiceDate { get; set; }

        public required string Counselor { get; set; }

        public required string StartTime { get; set; }

        public required string EndTime { get; set; }

        public required string Other { get; set; }

        public required string TreatmentType { get; set; }

    }
}
