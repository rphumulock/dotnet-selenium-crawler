using NRules;
using NRules.Fluent;
using Serilog;
using System.Reflection;

namespace HAI_Selenium.Services.NRules
{
    public class NRulesService : INRulesService
    {
        private readonly ISessionFactory _sessionFactory;

        public NRulesService()
        {
            try
            {
                // Initialize the rules repository
                var repository = new RuleRepository();

                // Load rules from the current assembly
                repository.Load(x => x.From(Assembly.GetExecutingAssembly()));
                Log.Information("Rules loaded successfully from assembly.");

                // Compile the rules into a session factory
                _sessionFactory = repository.Compile();
                Log.Information("Rules compiled successfully into a session factory.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize NRulesService.");
                throw;
            }
        }

        public ISession CreateSession()
        {
            try
            {
                // Create a session for executing rules
                var session = _sessionFactory.CreateSession();
                Log.Information("NRules session created successfully.");
                return session;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create NRules session.");
                throw;
            }
        }

        public void ExecuteRules(params object[] facts)
        {
            var session = CreateSession();

            try
            {
                // Insert all provided facts into the session
                foreach (var fact in facts)
                {
                    session.Insert(fact);
                    Log.Information($"Fact inserted into session: {fact}");
                }

                // Fire the rules to process the inserted facts
                session.Fire();
                Log.Information("Rules fired successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while executing rules.");
                throw;
            }
        }
    }
}
