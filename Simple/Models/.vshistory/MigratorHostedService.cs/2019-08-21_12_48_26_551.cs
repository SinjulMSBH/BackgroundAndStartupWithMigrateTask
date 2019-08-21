using Microsoft.Extensions.Hosting;

using Simple.Data;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{
    public class MigratorHostedService : IHostedService
    {
        // We need to inject the IServiceProvider so we can create
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        public MigratorHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var myDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //Do the migration asynchronously
                await myDbContext.Database.MigrateAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // noop
            return Task.CompletedTask;
        }
    }
}
