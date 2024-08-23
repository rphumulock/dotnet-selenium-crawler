namespace HAI_Selenium.InternalClasses.Request
{
    internal class FormDataForProcessing
    {

        public required PatientFormData patientFormData;
        public required List<List<ServiceDateFormData>> serviceDatesFormData;
    }
}
