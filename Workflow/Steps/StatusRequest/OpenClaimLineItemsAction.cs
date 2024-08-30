//using OpenQA.Selenium;
//using Serilog;

//namespace HAI_Selenium.Workflow.Steps.StatusRequest
//{
//    internal class OpenClaimsLineItemsAction : WorkflowStepBase
//    {
//        protected WorkflowContext Context { get; init; }

//        internal OpenClaimsLineItemsAction(WorkflowContext context)
//        {
//            Context = context;
//        }

//        protected override void PerformStep(IWebDriver driver)
//        {
//            try
//            {
//                Log.Information("[ACTION] Expanding claim line items...");

//                IWebElement openClaimLineItemsElement = Context.Get<IWebElement>("OpenClaimLineItemsElement");
//                IWebElement openLineItemsButton = openClaimLineItemsElement.FindElement(By.CssSelector("a[id*='claims-link']"));
//                openLineItemsButton.Click();

//                Log.Information("[SUCCESS] Claim line items expanded.");
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex, "An unexpected error occurred while processing form: {Message}", ex.Message);
//                throw;
//            }
//        }
//    }
//}
