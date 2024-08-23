using HAI_Selenium.InternalClasses.Status;

internal class ClaimDetails
{
    public ClaimData ClaimData { get; set; }
    public List<ClaimLineItem> ClaimLineItems { get; set; }

    public ClaimDetails(ClaimData claimData, List<ClaimLineItem> claimLineItems)
    {
        ClaimData = claimData;
        ClaimLineItems = claimLineItems;
    }

    public override string ToString()
    {
        var lineItemsInfo = string.Join("\n", ClaimLineItems.Select(item => item.ToString()));
        return $"{ClaimData}\nLine Items:\n{lineItemsInfo}";
    }
}
