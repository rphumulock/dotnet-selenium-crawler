using Newtonsoft.Json;

namespace HAI_Selenium.InternalClasses.CreateRequest
{
    /// <summary>
    /// Used to calculate the payment.
    /// </summary>
    internal class PaymentData
    {
        public required Dictionary<string, string> Intensive { get; set; }
        public required Dictionary<string, string> General { get; set; }
        public string GetAmount(string treatmentType, int minVisits)
        {
            Dictionary<string, string> selectedTreatmentType;

            switch (treatmentType)
            {
                case "Intensive":
                    selectedTreatmentType = Intensive;
                    break;
                case "General":
                    selectedTreatmentType = General;
                    break;
                default:
                    throw new ArgumentException($"Invalid treatment type: {treatmentType}");
            }

            if (selectedTreatmentType.TryGetValue(minVisits.ToString(), out var amount))
            {
                return amount;
            }
            else
            {
                throw new KeyNotFoundException($"No amount found for {treatmentType} with {minVisits} visits.");
            }
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
