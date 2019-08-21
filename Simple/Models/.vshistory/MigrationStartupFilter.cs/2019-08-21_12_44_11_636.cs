using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Simple.Data;

using System;

namespace Simple.Models
{
    public class MigratorStartupFilter : IStartupFilter
    {
        // We need to inject the IServiceProvider so we can create
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        public MigratorStartupFilter(IServiceProvider serviceProvider)
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
}
