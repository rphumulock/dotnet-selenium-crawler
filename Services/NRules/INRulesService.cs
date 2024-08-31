using NRules;

namespace HAI_Selenium.Services.NRules
{
    public interface INRulesService
    {
        ISession CreateSession();
        void ExecuteRules(params object[] facts);
    }
}
