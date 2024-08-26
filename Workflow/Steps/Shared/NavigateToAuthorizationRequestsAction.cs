namespace HAI_Selenium.Workflow.Steps.Shared
{
    internal class NavigateToAuthorizationRequestsAction : NavigationAction
    {
        protected override string DropdownSelector => "a[data-udfname='Authorization']";
        protected override string LinkSelector => "a[data-udfname='Requests']";
        protected override string DropdownName => "Authorization";
        protected override string LinkName => "Requests";
    }
}