using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.Request
{
    internal class ServiceRequest
    {
        public required string ID { get; set; }
        public required string SRID { get; set; }
        public required string SRAuth { get; set; }
        public required string AuthApprov { get; set; }
        public required string AuthStatus { get; set; }
        public required string ProvSite { get; set; }
        public required string Phone { get; set; }
        public required string Procedure { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required string Units { get; set; }
        public required string SubmissionDate { get; set; }
        public required string ModifiedDate { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}