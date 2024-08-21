using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses
{
    internal class ClaimLineItem
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
