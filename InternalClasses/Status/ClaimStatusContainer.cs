using HAI_Selenium.InternalClasses.Status;
using Newtonsoft.Json;

internal class ClaimStatusContainer(ClaimContainer claim, List<ClaimLineItemContainer> claimLineItems)
{
    public ClaimContainer Claim { get; set; } = claim;
    public List<ClaimLineItemContainer> ClaimLineItems { get; set; } = claimLineItems;
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
