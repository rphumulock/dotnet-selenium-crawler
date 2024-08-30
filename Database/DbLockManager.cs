using Medallion.Threading.Postgres;
using Npgsql;
using Serilog;

namespace HAI_Selenium.Utilities
{
    public class DatabaseLockManager : IDisposable
    {
        private readonly string _connectionString;
        private readonly string _clientId;
        private IDisposable _lockHandle;
        private NpgsqlConnection _connection;

        public DatabaseLockManager(string connectionString, string clientId)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        }

        public void AcquireLock()
        {
            try
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
                Log.Information("Connected to PostgreSQL database.");

                var lockId = new PostgresAdvisoryLockKey(_clientId.GetHashCode());
                var dbLock = new PostgresDistributedLock(lockId, _connection);

                Log.Information("Attempting to acquire lock for client ID: {ClientId}", _clientId);
                _lockHandle = dbLock.Acquire();
                Log.Information("Lock acquired for client ID: {ClientId}", _clientId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while acquiring the lock for client ID: {ClientId}", _clientId);
                Dispose(); // Ensure proper cleanup
                throw; // Re-throw the exception to allow handling upstream
            }
        }

        public void ReleaseLock()
        {
            try
            {
                _lockHandle?.Dispose();
                _connection?.Close();
                Log.Information("Lock released and database connection closed for client ID: {ClientId}", _clientId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while releasing the lock for client ID: {ClientId}", _clientId);
            }
        }

        public void Dispose()
        {
            ReleaseLock();
            _connection?.Dispose();
            _lockHandle = null;
            _connection = null;
        }
    }
}
