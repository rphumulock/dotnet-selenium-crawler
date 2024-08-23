public class NavigateToClaimsStatusAction : NavigationAction
{
    protected override string DropdownSelector => "a[data-udfname='Claims']";
    protected override string LinkSelector => "a[data-udfname='Claim Status']";
    protected override string DropdownName => "Claims";
    protected override string LinkName => "Claim Status";
}
