using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Options;
using Timer = System.Timers.Timer;

namespace BlazorTestV2.Service
{
    public sealed class IdleCircuitHandler : CircuitHandler, IDisposable
    {
        private Circuit? currentCircuit;
        private readonly ILogger logger;
        private readonly Timer timer;
        private readonly AuthenticationStateProvider provider;

        public IdleCircuitHandler(ILogger<IdleCircuitHandler> logger,
            IOptions<IdleCircuitOptions> options, AuthenticationStateProvider stateProvider)
        {
            timer = new Timer
            {
                Interval = options.Value.IdleTimeout.TotalMilliseconds,
                AutoReset = false
            };

            timer.Elapsed += CircuitIdle;
            this.logger = logger;
            this.provider = stateProvider;
        }

        /// Circuit Idle
        private void CircuitIdle(object? sender, System.Timers.ElapsedEventArgs e)
        {
            logger.LogInformation("{CircuitId} is idle", currentCircuit?.Id);    //記錄 Circuit.Id 停用時間
        }


        /// Circuit start
        public override Task OnCircuitOpenedAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            currentCircuit = circuit;
            logger.LogInformation("{CircuitId} start.", currentCircuit?.Id);    //記錄 Circuit.Id 出生時間
            #region Circuit.Id 寫入 LocalStorage
            MyAuthenticationStateProvider myProvider = (MyAuthenticationStateProvider)provider;
            myProvider.SetUniqueID(circuit.Id);
            #endregion
            return Task.CompletedTask;
        }

        public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
            Func<CircuitInboundActivityContext, Task> next)
        {
            return context =>
            {
                timer.Stop();
                timer.Start();

                return next(context);
            };
        }

        public void Dispose()
        {
            logger.LogInformation("{CircuitId} Dispose.", currentCircuit?.Id);    //記錄 Circuit.Id 死亡時間
            timer.Dispose();
        }

        public string? GetCircuitId()
        {
            return currentCircuit?.Id;
        }
    }

    public class IdleCircuitOptions
    {
        public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(20);    // Idle Timeout 時間
    }

    public static class IdleCircuitHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdleCircuitHandler(
            this IServiceCollection services,
            Action<IdleCircuitOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddIdleCircuitHandler();

            return services;
        }

        public static IServiceCollection AddIdleCircuitHandler(
            this IServiceCollection services)
        {
            services.AddScoped<CircuitHandler, IdleCircuitHandler>();

            return services;
        }
    }
}
