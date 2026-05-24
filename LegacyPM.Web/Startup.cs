using LegacyPM.Core;
using LegacyPM.Core.Services;
using LegacyPM.Web.BackgroundServices;
using LegacyPM.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LegacyPM.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ProjectDb")));
            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddHostedService<DeadlineReminderBackgroundService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<ReportCacheService>();
            services.AddSingleton<ExternalHttpService>();
            services.AddTransient<EmailReminderService>();
            services.AddTransient<CultureService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}