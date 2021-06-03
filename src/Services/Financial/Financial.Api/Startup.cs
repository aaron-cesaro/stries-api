using Financial.Api.Interfaces;
using Financial.Api.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace Financial.Api
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
            Log.Information($"Configuring {typeof(Startup).GetTypeInfo().Assembly.GetName().Name} services");

            // Service custom Extensions
            services
                .AddStriesServices()
                .AddHostedServices();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Financial.Api", Version = "v1" });
            });

            Log.Information($"{typeof(Startup).GetTypeInfo().Assembly.GetName().Name} services configured");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Information($"Configuring {typeof(Startup).GetTypeInfo().Assembly.GetName().Name}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financial.Api v1");
                    c.RoutePrefix = string.Empty;
                });

                Log.Information(
                    $"{typeof(Startup).GetTypeInfo().Assembly.GetName().Name} is using {env.EnvironmentName} enviroment");
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Log.Information($"{typeof(Startup).GetTypeInfo().Assembly.GetName().Name} configured");
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddStriesServices(this IServiceCollection services)
        {
            // Application
            services.AddSingleton<IFinancialManager, FinancialManager>();

            return services;
        }

        // Add Hosted Services for background tasks
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
