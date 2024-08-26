using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.Request
{
    internal class RequestInvoiceFormHeaderData
    {
        public required string authNumber;
        public required string patientPolicyNumber;
        public required List<string> patientDiagnosisCodes;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}