namespace HAI_Selenium.Utilities
{
    public class RecoverableError : Exception
    {
        public RecoverableError(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RecoverableError(string message)
            : base(message)
        {
        }
    }

    public class NonRecoverableError : Exception
    {
        public NonRecoverableError(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NonRecoverableError(string message)
            : base(message)
        {
        }
    }
}
