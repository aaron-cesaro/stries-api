using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Post.Database.Contextes;
using System;
using System.IO;

namespace Post.Infrastructure.Factories
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

            optionsBuilder.UseNpgsql(config.GetConnectionString("PostContext"), npgsqlOptionsAction: o => o.MigrationsAssembly("Post"));

            return new PostContext(optionsBuilder.Options);
        }
    }
}
