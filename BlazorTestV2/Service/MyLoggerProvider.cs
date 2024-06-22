namespace BlazorTestV2.Service
{
    public sealed class MyLoggerProvider : ILoggerProvider
    {
        private ILogger logger;

        public MyLoggerProvider() 
        {
            logger = new MyLogger();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return logger;
        }

        public void Dispose() {}

    }
}
