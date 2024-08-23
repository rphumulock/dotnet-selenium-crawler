using HAI_Selenium.InternalClasses.Invoice;
using OpenQA.Selenium;

internal class GetServiceMonthAction : WorkflowStepBase
{
    private readonly RequestInvoice _invoice;
    private readonly WorkflowContext _context;

    protected GetServiceMonthAction(RequestInvoice invoice, WorkflowContext context)
    {
        _invoice = invoice;
        _context = context;
    }

    protected override void PerformStep(IWebDriver driver)
    {
        Console.WriteLine("[ACTION] Validating service dates month ...");

        if (_invoice.ServiceDateRequests == null || _invoice.ServiceDateRequests.Count == 0)
        {
            throw new ArgumentNullException("[ERROR] ServiceDateRequests cannot be null or empty.");
        }

        string serviceDateMonth = null;

        foreach (var serviceDateRequest in _invoice.ServiceDateRequests)
        {
            string[] dateParts = serviceDateRequest.ServiceDate.Split('/');
            string currentMonth = dateParts[0];

            if (serviceDateMonth == null)
            {
                serviceDateMonth = currentMonth;
            }
            else if (currentMonth != serviceDateMonth)
            {
                throw new InvalidOperationException($"[ERROR] Mismatch found: expected month {serviceDateMonth}, but found {currentMonth}.");
            }
        }

        // Store the validated service date month in the context
        _context.Set("ServiceDateMonth", serviceDateMonth);

        Console.WriteLine($"[SUCCESS] Invoice data loaded and service month validated: {serviceDateMonth}.");
    }
}
