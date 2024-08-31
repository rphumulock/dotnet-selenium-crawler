using Newtonsoft.Json;
using System.Security.Claims;

namespace HAI_Selenium.InternalClasses.StatusRequest
{
    /// <summary>
    /// Structure for the Status of a Claim that contains Line Items.
    /// </summary>
    internal class ClaimStatus
    {
        public required string ClaimNumber { get; set; }
        public required string NPI { get; set; }
        public required string Member { get; set; }
        public required string Provider { get; set; }
        public required string Status { get; set; }
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
   .Match<Claim>(() => claim,
                c => c.ClaimStatus == "Not Adjudicated",
                c => c.ServiceDateStatus == "Not Adjudicated",
                c => c.AmountRequested == 1200.00m,  // Match decimal value, not formatted string
                c => c.AmountPaid == 0.00m);  // Zero represents "$ -" in numeric form