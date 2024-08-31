using NRules;
using NRules.Fluent;
using System.Reflection;

namespace HAI_Selenium.Services
{
    public class NRulesService : INRulesService
    {
        private readonly ISessionFactory _sessionFactory;

        public NRulesService()
        {
            // Initialize the rules repository
            var repository = new RuleRepository();

            // Load rules from the current assembly
            repository.Load(x => x.From(Assembly.GetExecutingAssembly()));

            // Compile the rules into a session factory
            _sessionFactory = repository.Compile();
        }

        public ISession CreateSession()
        {
            // Create a session for executing rules
            return _sessionFactory.CreateSession();
        }
    }
}
