namespace HAISelenium.InternalClasses
{
    internal class Invoice
    {
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string policyNumber { get; set; }
        public required string diagnosisCode { get; set; }
        public required string dob { get; set; }
        public required string providerID { get; set; }
        public required string gender { get; set; }
        public required List<ServiceDateData> serviceDates { get; set; }
        public override string ToString()
        {
            return $"FirstName: {firstName}," +
                $" LastName: {lastName}," +
                $" PolicyNumber: {policyNumber}," +
                $" DiagnosisCode: {diagnosisCode}," +
                $" DateOfBirth: {dob}," +
                $" ProviderID: {providerID}," +
                $" Gender: {gender}";
        }
    }
}
