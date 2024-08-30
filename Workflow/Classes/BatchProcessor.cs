using HAI_Selenium.Database.Models;
using HAI_Selenium.InternalClasses.CreateRequest;
using HAI_Selenium.Workflow.Classes;
using HAI_Selenium.Workflow.Steps.CreateRequest;
using OpenQA.Selenium;
using Serilog;

internal class BatchProcessor
{
    private readonly int _index;
    private readonly WorkflowContext _context;
    private readonly List<ClaimServiceDateFormData> _serviceDateFormData;
    private readonly ICollection<ServiceDateRequest> _serviceDateRequests;
    private bool _isProcessed;

    public BatchProcessor(int index, WorkflowContext context, List<ClaimServiceDateFormData> serviceDateFormData, ICollection<ServiceDateRequest> serviceDateRequests)
    {
        _index = index;
        _context = context;
        _serviceDateFormData = serviceDateFormData;
        _serviceDateRequests = serviceDateRequests;
        _isProcessed = false;
    }

    public bool IsProcessed => _isProcessed;

    public async Task ProcessAsync(IWebDriver driver)
    {
        if (_isProcessed)
        {
            Log.Information("Skipping already processed batch {BatchIndex}.", _index);
            return;
        }

        try
        {
            SetBatchContext();
            await ExecuteProcessFormDataChain(driver);
            _isProcessed = true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing batch {BatchIndex}.", _index);
            throw;
        }
    }

    private void SetBatchContext()
    {
        _context.Set("CurrentBatchServiceDateFormData", _serviceDateFormData);
        _context.Set("RemainingBatchesServiceDateFormData", null); // Optionally manage this context setting

        _context.Set("CurrentBatchServiceDateRequests", _serviceDateRequests);
        _context.Set("RemainingBatchesServiceDateRequests", null); // Optionally manage this context setting
    }

    private async Task ExecuteProcessFormDataChain(IWebDriver driver)
    {
        var processFormDataChain = new WorkflowChain()
            .AddStep(new CaptureButtonsAction(_context))
            .AddStep(new AddClaimAction(_context))
            .AddStep(new ProcessClaimFormHeaderAction(_context))
            .AddStep(new ProcessFormServiceDatesAction(_context, _index))
            .AddStep(new ProcessClaimFormFooterAction(_context))
            .AddStep(new CancelClaimAction(_context));

        try
        {
            await processFormDataChain.ExecuteAsync(driver);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing workflow chain.");
            throw;
        }
    }
}
