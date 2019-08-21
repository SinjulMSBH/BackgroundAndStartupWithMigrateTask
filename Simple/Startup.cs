using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Simple.Data;
using Simple.Models;

namespace Simple
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            //svcs.AddMvc(opt =>
            //{
            //    if (_env.IsProduction())
            //    {
            //        opt.Filters.Add(new RequireHttpsAttribute());
            //    }
            //    opt.EnableEndpointRouting = false;
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Add migration task
            services.AddStartupTask<MigrationStartupTask>();

            //services.AddAsyncInitializer<MyAppInitializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseAuthentication();
            //app.UseAuthorization();
            //app.UseHttpsRedirection() // <--

            // Old Version
            void CreateRoutes(IRouteBuilder routes)
            {
                routes.MapRoute(
                  name: "Events",
                  template: string.Concat("{moniker}/{controller=Root}/{action=Index}/{id?}")
                  );

                routes.MapRoute(
                  name: "Default",
                  template: "{controller=Root}/{action=Index}/{id?}"
                  );

            }
            //app.UseMvc(CreateRoutes);

            void CreateRoutes2(IEndpointRouteBuilder enpoints) // <--
            {
                enpoints.MapControllerRoute( // <--
                  "Events",
                  string.Concat("{moniker}/{controller=Root}/{action=Index}/{id?}")
                  );

                enpoints.MapControllerRoute( // <--
                   "Default",
                  "{controller=Root}/{action=Index}/{id?}"
                );

            }

            app.UseEndpoints(endpoints =>
            {
                //CreateRoutes2(endpoints);
                //this.CreateRoutes2(endpoints);
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            //The only other change I needed to make was to install the Entity Framework tools globally on my machine.In ASP.NET Core 3.0, it's not installed by default or at the project level. To do this, open a console and type:
            //dotnet tool install --global dotnet-ef --version 3.0.0-*
        }
    }
}
