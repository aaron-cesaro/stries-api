using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Post.Application.EventHandlers;
using Post.Infrastructure.MessageBroker;
using Post.Interfaces;
using Post.Managers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            // Add routing keys based on headers for message broker
            var routingHeaders = new Dictionary<string, object>
            {
                { "", ""}
            };

            // Service custom Extensions
            services
                .ConfigureMessageBroker(Configuration, routingHeaders)
                .AddStriesServices()
                .AddHostedServices();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Post", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Post v1"));
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
    }
}
