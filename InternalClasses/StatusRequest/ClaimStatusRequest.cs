namespace HAI_Selenium.InternalClasses.StatusRequest
{
    /// <summary>
    /// Structure for each Claim in the Invoice Status Request.
    /// </summary>
    internal class ClaimStatusRequest
    {
        public required string ClaimID { get; set; }
        public required string DateReceived { get; set; }
    }

}
