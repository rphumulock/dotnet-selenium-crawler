using Newtonsoft.Json;
using System.Security.Claims;

namespace HAI_Selenium.InternalClasses.StatusRequest
{
    /// <summary>
    /// Structure for each Line Item nested under a Claim.
    /// </summary>
    internal class ClaimStatusLineItem
    {

        public required string LineItemControlNumber { get; set; }

        public required string ServiceDates { get; set; }

        public required string Service { get; set; }

        public required string ServiceProcedureModifiers { get; set; }

        public required string ServiceStatus { get; set; }

        public required string Charge { get; set; }

        public required string Approved { get; set; }

        public required string Units { get; set; }

        public required string ExceptionAdjudicationReason { get; set; }

        public required string ReAdjudicationReason { get; set; }

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