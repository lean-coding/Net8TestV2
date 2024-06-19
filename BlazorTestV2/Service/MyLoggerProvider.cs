namespace BlazorTestV2.Service
{
    public class MyLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(); 
        }

        public void Dispose() { }

    }
}
