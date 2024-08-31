using HAI_Selenium.InternalClasses.StatusRequest;
using NRules.Fluent.Dsl;

public class ClaimNotAdjudicatedRule : Rule
{
    public override void Define()
    {
        ClaimStatus claimStatus = null;

        // Define the conditions for the rule
        When()
            .Match<ClaimStatus>(() => claimStatus,
                c => c.ClaimStatus == "Not Adjudicated",
                c => c.ServiceDateStatus == "Not Adjudicated",
                c => c.AmountRequested == 1200.00m,  // Match decimal value, not formatted string
                c => c.AmountPaid == 0.00m);  // Zero represents "$ -" in numeric form

        // Define the action to take when the conditions are met
        Then()
            .Do(ctx => claim.EndStatus = "Not Adjudicated");
    }
}
