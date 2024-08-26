namespace HAI_Selenium.InternalClasses.StatusRequest
{
    internal class InvoiceStatusRequest
    {
        /// <summary>
        /// Structure for a Status Request on and Invoice that contains multiple Claims.
        /// </summary>
        public required string InvoiceID { get; set; }
        public required List<ClaimStatusRequest> ClaimStatusRequests { get; set; }
    }
}