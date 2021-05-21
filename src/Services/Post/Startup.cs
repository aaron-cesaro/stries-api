using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Post.Application.EventHandlers;
using Post.Database.Contextes;
using Post.Infrastructure.MessageBroker;
using Post.Interfaces;
using Post.Managers;
using Post.Repositories;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Post
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
                .AddCustomDbContext(Configuration)
                .ConfigureMessageBroker(Configuration)
                .AddHostedServices();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Post", Version = "v1" });
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
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Post v1");
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
        // Message Broker configuration
        public static IServiceCollection ConfigureMessageBroker(this IServiceCollection services, IConfiguration Configuration)
        {
            // Add routing keys based on headers for message broker
            var routingHeaders = new Dictionary<string, object>
            {
                { "PostCreated", "PostCreated"},
                { "PostPublished", "PostPublished"},
                { "PostDeleted", "PostDeleted"}
            };

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

        public static IServiceCollection AddStriesServices(this IServiceCollection services)
        {

            // Application
            services.AddSingleton<IPostManager, PostManager>();
            services.AddSingleton<IPostRepository, PostRepository>();

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
            services.AddDbContext<PostContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostContext"),
                    npgsqlOptionsAction: npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                    });
            },
            ServiceLifetime.Singleton);

            return services;
        }
    }
}
