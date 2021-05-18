using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using User.Application.EventHandlers;
using User.Database.Contextes;
using User.Infrastructure.MessageBroker;
using User.Interfaces;
using User.Managers;

namespace User
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

            // Add routing keys based on headers for message broker
            var routingHeaders = new Dictionary<string, object>
            {
                { "", ""}
            };

            // Service custom Extensions
            services
                .AddCustomDbContext(Configuration)
                .ConfigureMessageBroker(Configuration, routingHeaders)
                .AddStriesServices()
                .AddHostedServices();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User", Version = "v1" });
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User v1"));

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
        // Message Broker configuration
        public static IServiceCollection ConfigureMessageBroker(this IServiceCollection services, IConfiguration Configuration, Dictionary<string, object> routingHeaders)
        {
            // Get message broker settings from configuration
            var messageBrokerSettings = Configuration.GetSection("MessageBrokerSettings");

            // Add connection provider with specified container url
            services.AddSingleton<IConnectionProvider>(new ConnectionProvider(messageBrokerSettings.GetValue<string>("Url")));

            services.AddSingleton<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
                    "stries_default_exchange",
                    ExchangeType.Headers));

            services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
                "stries_default_exchange",
                "stries_default_queue",
                routingHeaders,
                ExchangeType.Headers));

            return services;
        }

        // Add Stries Services
        public static IServiceCollection AddStriesServices(this IServiceCollection services)
        {
            services.AddSingleton<ITemplateManager, TemplateManager>();

            return services;
        }

        // Add Hosted Services for background tasks
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<TemplateHandler>();

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("UserContext"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            },
            ServiceLifetime.Scoped);

            return services;
        }
    }
}
