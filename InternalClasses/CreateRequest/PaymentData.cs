using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.StatusRequest
{
    /// <summary>
    /// Structure for calculating the Payment
    /// </summary>
    internal class PaymentData
    {
        public required int ServiceDatesCount { get; set; }

        public required string TreatmentType { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}