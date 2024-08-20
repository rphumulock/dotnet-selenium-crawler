namespace HAISelenium.InternalClasses
{
    internal class Invoice
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PolicyNumber { get; set; }
        public required string DiagnosisCode { get; set; }
        public required string DoB { get; set; }
        public required string ProviderID { get; set; }
        public required string Gender { get; set; }
        public required List<ServiceDateRequest> ServiceDateRequests { get; set; }
        public override string ToString()
        {
            return $"FirstName: {FirstName}," +
                $" LastName: {LastName}," +
                $" PolicyNumber: {PolicyNumber}," +
                $" DiagnosisCode: {DiagnosisCode}," +
                $" DateOfBirth: {DoB}," +
                $" ProviderID: {ProviderID}," +
                $" Gender: {Gender}";
        }
    }
}