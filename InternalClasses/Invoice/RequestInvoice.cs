using HAI_Selenium.InternalClasses.Request;

namespace HAI_Selenium.InternalClasses.Invoice
{
    internal class RequestInvoice
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PolicyNumber { get; set; }
        public required List<string> DiagnosisCodes { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required List<ServiceDateRequest> ServiceDateRequests { get; set; }
        public override string ToString()
        {
            return $"FirstName: {FirstName}," +
                $" LastName: {LastName}," +
                $" PolicyNumber: {PolicyNumber}," +
                $" DiagnosisCode: {DiagnosisCodes.ToString()}," +
                $" DateOfBirth: {DateOfBirth}," +
                $" Gender: {Gender}";
        }
    }
}