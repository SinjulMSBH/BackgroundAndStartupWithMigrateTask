using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{

    public class MigrationStartupTask : IStartupTask
    {
        // We need to inject the IServiceProvider so we can create
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        public MigrationStartupTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // Create a new scope to retrieve scoped services
            using var scope = _serviceProvider.CreateScope();
            // Get the DbContext instance
            var myDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //Do the migration
            await myDbContext.Database.MigrateAsync();
        }

        public Task ShutdownAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
