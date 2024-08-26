using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.CreateRequest
{
    /// <summary>
    /// Data for each Service date in Claim form.
    /// </summary>
    internal class ClaimServiceDateFormData
    {
        public required string StartDate { get; set; }
        public required string PlaceOfService { get; set; }
        public required string CPT { get; set; }
        public required string DiagnosisPointer { get; set; }
        public required string ChargesDollars { get; set; }
        public required string ChargesCents { get; set; }
        public required string Units { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

