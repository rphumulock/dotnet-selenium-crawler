using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.CreateRequest
{
    internal class CreateClaimsRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PolicyNumber { get; set; }
        public required List<string> DiagnosisCodes { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required List<ServiceDateRequests> ServiceDateRequests { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}