using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Simple.Data;
using Simple.Models;

using System.Threading.Tasks;

namespace Simple
{
    public class Program
    {
        public static void Main1(string[] args)
        {
            //note that migrating during startup is for DEMO only
            //in production I'd migrate outside of the app
            //also there's a small chance of conflict if this is hosted
            //in multiple places and migrate is hit at the same moment.
            CreateHostBuilder(args).Build().MigrateDatabase<ApplicationDbContext>().Run();
        }

        public static async Task Main2(string[] args)
        {
            IHost webHost = CreateHostBuilder(args).Build();

            // Create a new scope
            using (var scope = webHost.Services.CreateScope())
            {
                // Get the DbContext instance
                var myDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //Do the migration asynchronously
                await myDbContext.Database.MigrateAsync();
            }

            // Run the WebHost, and start accepting requests
            // There's an async overload, so we may as well use it
            await webHost.RunAsync();
        }

        public static async Task Main3(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //await host.InitAsync(); //Ver 2.2
            host.Run();
        }

        // Change return type from void to async Task
        public static async Task Main(string[] args)
        {
            // CreateWebHostBuilder(args).Build().Run();
            await CreateHostBuilder(args).Build().RunWithTasksAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService() //Microsoft.Extensions.Hosting.WindowsServices
                                     //cs create Simple binPath=C:\Path\To\Simple.exe
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices((hostContext, services) =>
                        {
                            services.AddHttpClient();
                            services.AddHostedService<Worker>();
                        })
                        //.ConfigureKestrel(o =>
                        //{
                        //    o.ListenUnixSocket("/var/listen.sock");
                        //})
                        .UseStartup<Startup>()
                    ;
                });
    }
}
