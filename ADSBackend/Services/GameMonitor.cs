using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADSBackend.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ADSBackend.Services
{
    public class GameMonitor : HostedService
    {
        private readonly IServiceProvider _provider;

        public GameMonitor(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                using (IServiceScope scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var config = scope.ServiceProvider.GetRequiredService<Configuration>();

                    try
                    {
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, ex.Message);

                        // if the DB is not currently connected, wait 15 seconds and try again
                        await Task.Delay(TimeSpan.FromSeconds(15));

                        continue;
                    }
                }

                var task = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

    }
}
