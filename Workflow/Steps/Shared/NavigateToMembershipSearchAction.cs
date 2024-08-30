using HAI_Selenium.Workflow.Classes;

namespace HAI_Selenium.Workflow.Steps.Shared
{

    internal class NavigateToMembershipSearchAction : NavigationAction
    {

        internal NavigateToMembershipSearchAction(WorkflowContext context) : base(context) { }

        protected override string DropdownSelector => "a[data-udfname='Membership']";

        protected override string LinkSelector => "a[data-udfname='Search']";

        protected override string DropdownName => "Membership";

        protected override string LinkName => "Search";

    }
}