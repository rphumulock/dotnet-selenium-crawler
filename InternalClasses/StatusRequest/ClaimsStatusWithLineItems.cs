using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.StatusRequest
{
    /// <summary>
    /// Struture for the compilation of a Claim Status with each of it's Line Items.
    /// </summary>
    /// <param name="claimStatus"></param>
    /// <param name="claimLineItems"></param>
    internal class ClaimsStatusWithLineItems(ClaimStatus claimStatus, List<ClaimStatusLineItem> claimLineItems)
    {
        public ClaimStatus ClaimStatus { get; set; } = claimStatus;
        public List<ClaimStatusLineItem> ClaimLineItems { get; set; } = claimLineItems;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}