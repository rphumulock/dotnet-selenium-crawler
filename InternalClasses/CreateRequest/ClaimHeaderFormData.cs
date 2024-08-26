using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.CreateRequest
{
    /// <summary>
    /// Data that goes into the top portion of the Claim creation form.
    /// </summary>
    internal class ClaimHeaderFormData
    {

        public required string AuthorizationNumber;

        public required string PolicyNumber;

        public required List<string> DiagnosisCodes;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}