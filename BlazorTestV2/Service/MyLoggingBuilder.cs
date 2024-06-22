using BlazorTestV2.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

public static class MyLoggerExtensions
{
    public static ILoggingBuilder AddMyLogger(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, MyLoggerProvider>());

        return builder;
    }
}