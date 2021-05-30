using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Post.Api.Application.EventHandlers;
using Post.Api.Database.Contextes;
using Post.Api.Infrastructure.Data;
using Post.Api.Infrastructure.MessageBroker;
using Post.Api.Interfaces;
using Post.Api.Managers;
using Post.Api.Repositories;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Reflection;

namespace Post.Api
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
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Post v1");
                    c.RoutePrefix = string.Empty;
                });

                DatabaseInitializer.DatabaseSeed(app);

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
            // Add routing keys for message broker
            var userCreatedRoutingKey = "user.created";
            var userUpdatedRoutingKey = "user.updated";
            var userDeletedRoutingKey = "user.deleted";

            // Get message broker settings from configuration
            var messageBrokerSettings = Configuration.GetSection("MessageBrokerSettings");

            // Add connection provider with specified container url
            services.AddSingleton<IConnectionProvider>(new ConnectionProvider(messageBrokerSettings.GetValue<string>("Url")));

            // Add exchange to publish events
            services.AddSingleton<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
                    "stries_post_exchange",
                    ExchangeType.Direct));

            // Add user Created subscription
            services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
            "stries_user_exchange",
            "stries_user_queue",
            userCreatedRoutingKey,
            ExchangeType.Direct));

            // Add user Updated subscription
            services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
            "stries_user_exchange",
            "stries_user_queue",
            userUpdatedRoutingKey,
            ExchangeType.Direct));

            // Add user Created subscription
            services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
            "stries_user_exchange",
            "stries_user_queue",
            userDeletedRoutingKey,
            ExchangeType.Direct));

            return services;
        }

        public static IServiceCollection AddStriesServices(this IServiceCollection services)
        {

            // Application
            services.AddSingleton<IPostManager, PostManager>();
            services.AddSingleton<IPostRepository, PostRepository>();
            services.AddSingleton<IAuthorManager, AuthorManager>();
            services.AddSingleton<IAuthorRepository, AuthorRepository>();

            return services;
        }

        // Add Hosted Services for background tasks
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<UserCreatedEventHandler>();
            services.AddHostedService<UserUpdatedEventHandler>();
            services.AddHostedService<UserDeletedEventHandler>();

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
