namespace HAISelenium.InternalClasses
{
    internal class ServiceDateData
    {
        public required string serviceDate { get; set; }
        public required string counselor { get; set; }
        public required string startTime { get; set; }
        public required string endTime { get; set; }
        public string? other { get; set; }
        public override string ToString()
        {
            return $"Counselor: {counselor}, ServiceDate: {serviceDate}, StartTime: {startTime}, EndTime: {endTime}";
        }
    }
}