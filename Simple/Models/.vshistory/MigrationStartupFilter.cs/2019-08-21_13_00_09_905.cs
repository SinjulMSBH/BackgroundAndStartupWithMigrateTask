using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Simple.Data;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{
    /// <summary>
    /// Warning: this code uses bad async practices.
    /// It's very possible that this won't cause any issues - this code is only running on app startup, before we're serving requests, and so deadlocks seem unlikely. But frankly, I couldn't say for sure and I avoid code like this wherever possible.
    /// </summary>
    public class BadMigratorStartupFilter : IStartupFilter
    {
        // We need to inject the IServiceProvider so we can create
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        public BadMigratorStartupFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var myDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //Do the migration by blocking the async call
                myDbContext.Database.MigrateAsync()
                    .GetAwaiter()   // Yuk!
                    .GetResult();   // Yuk!
            }

            // don't modify the middleware pipeline
            return next;
        }
    }

    public class MigratorStartupFilter : IStartupTask
    {
        // We need to inject the IServiceProvider so we can create
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        public MigratorStartupFilter(IServiceProvider serviceProvider)
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
    }
}
