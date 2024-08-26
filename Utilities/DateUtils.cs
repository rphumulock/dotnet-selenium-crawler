using System.Globalization;
using HAI_Selenium.InternalClasses.CreateRequest;

namespace HAI_Selenium.Utilities
{
    internal static class DateUtils
    {
        internal static ServiceDateRequests FindLatestServiceDate(List<ServiceDateRequests> serviceDateRequests)
        {
            string[] formats = { "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy" };

            return serviceDateRequests
                .Select(serviceDateRequest =>
                {
                    if (!DateTime.TryParseExact(serviceDateRequest.ServiceDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        throw new InvalidOperationException($"Invalid date format: {serviceDateRequest.ServiceDate}");
                    }
                    return new { serviceDateRequest, parsedDate };
                })
                .OrderByDescending(x => x.parsedDate)
                .First()
                .serviceDateRequest;
        }

        public static string GetLastDayOfMonth(string monthString, string yearString)
        {
            if (!int.TryParse(monthString, out int month) || month is < 1 or > 12)
            {
                throw new ArgumentException("Invalid month format. Please enter a valid month as a number (e.g., '5' or '05').");
            }

            if (!int.TryParse(yearString, out int year))
            {
                throw new ArgumentException("Invalid year format. Please enter a valid year as a number (e.g., '2023').");
            }

            return DateTime.DaysInMonth(year, month).ToString();
        }

        public static string RemoveLeadingZero(string input)
        {
            return input.StartsWith("0") && input.Length > 1 ? input.Substring(1) : input;
        }
    }
}
