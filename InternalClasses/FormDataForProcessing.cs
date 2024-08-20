using HAISelenium.InternalClasses;

namespace HAI_Selenium.InternalClasses
{
    internal class FormDataForProcessing
    {
     
        public required PatientFormData patientFormData;
        public required List<List<ServiceDateFormData>> serviceDatesFormData;
    }
}
