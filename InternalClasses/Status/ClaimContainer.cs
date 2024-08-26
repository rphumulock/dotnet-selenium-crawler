using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.Status
{
    internal class ClaimContainer
    {
        public required string ClaimNumber { get; set; }
        public required string NPI { get; set; }
        public required string Member { get; set; }
        public required string Provider { get; set; }
        public required string ClaimStatus { get; set; }
        public required string DateReceived { get; set; }
        public required string TotalCharge { get; set; }
        public required string TotalApproved { get; set; }
        public required string ServiceDates { get; set; }
        public required string BatchNumber { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
