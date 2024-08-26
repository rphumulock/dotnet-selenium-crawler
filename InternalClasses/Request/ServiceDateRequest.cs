using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.Request
{
    internal class ServiceDateRequest
    {
        public required string ServiceDate { get; set; }
        public required string Counselor { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public required string TreatmentType { get; set; }
        public string? Other { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
