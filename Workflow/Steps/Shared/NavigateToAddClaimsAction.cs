using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.Shared
{
    internal class NavigateToAddClaimsAction : NavigationAction
    {
        internal NavigateToAddClaimsAction(WorkflowContext context) : base(context) { }
        protected override string DropdownSelector => "a[data-udfname='Claims']";
        protected override string LinkSelector => "a[data-udfname='Add Claim']";
        protected override string DropdownName => "Claims";
        protected override string LinkName => "Add Claim";
    }
}