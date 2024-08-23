public class NavigateToAddClaimsAction : NavigationAction
{
    protected override string DropdownSelector => "a[data-udfname='Claims']";
    protected override string LinkSelector => "a[data-udfname='Add Claim']";
    protected override string DropdownName => "Claims";
    protected override string LinkName => "Add Claim";
}
