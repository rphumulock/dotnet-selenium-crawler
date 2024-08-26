internal class NavigateToMembershipSearchAction : NavigationAction
{
    protected override string DropdownSelector => "a[data-udfname='Membership']";
    protected override string LinkSelector => "a[data-udfname='Search']";
    protected override string DropdownName => "Membership";
    protected override string LinkName => "Search";
}
