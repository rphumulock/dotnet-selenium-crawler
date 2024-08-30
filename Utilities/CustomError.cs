using Newtonsoft.Json;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Exceptions
{
    internal class ClaimServiceDateException : Exception
    {

        public List<ClaimServiceDateFormData> ServiceDateFormDataList { get; }

        public ClaimServiceDateException(string message) : base(message)
        {
            ServiceDateFormDataList = new List<ClaimServiceDateFormData>();
        }

        public ClaimServiceDateException(string message, List<ClaimServiceDateFormData> serviceDateFormDataList)
            : base(message)
        {
            ServiceDateFormDataList = serviceDateFormDataList;
        }

        public ClaimServiceDateException(string message, Exception innerException, List<ClaimServiceDateFormData> serviceDateFormDataList)
            : base(message, innerException)
        {
            ServiceDateFormDataList = serviceDateFormDataList;
        }

        public override string ToString()
        {
            string baseString = base.ToString();
            string serviceDatesJson = JsonConvert.SerializeObject(ServiceDateFormDataList, Formatting.Indented);
            return $"{baseString}\nService Date Form Data: {serviceDatesJson}";
        }
    }
}
