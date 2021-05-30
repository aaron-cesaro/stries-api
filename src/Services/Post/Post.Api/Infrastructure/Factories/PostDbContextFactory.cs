using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Post.Api.Database.Contextes;
using System;
using System.IO;

namespace Post.Api.Infrastructure.Factories
{

    public class PostDbContextFactory : IDesignTimeDbContextFactory<PostContext>
    {
        public PostContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PostContext>();

            optionsBuilder.UseNpgsql(config.GetConnectionString("PostContext"), npgsqlOptionsAction: o => o.MigrationsAssembly("Post.Api"));

            return new PostContext(optionsBuilder.Options);
        }
    }
}
