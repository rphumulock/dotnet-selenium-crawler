using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    internal class NavigateToClaimsStatusAction : NavigationAction
    {
        internal NavigateToClaimsStatusAction(WorkflowContext context) : base(context)
        {
        }
        protected override string DropdownSelector => "a[data-udfname='Claims']";
        protected override string LinkSelector => "a[data-udfname='Claim Status']";
        protected override string DropdownName => "Claims";
        protected override string LinkName => "Claim Status";
    }
}
