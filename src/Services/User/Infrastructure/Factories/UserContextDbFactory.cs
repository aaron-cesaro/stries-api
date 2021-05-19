﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using User.Database.Contextes;

namespace User.Infrastructure.Factories
{
    public class PostDbContextFactory : IDesignTimeDbContextFactory<UserContext>
    {
        public UserContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<UserContext>();

            optionsBuilder.UseNpgsql(config.GetConnectionString("UserContext"), npgsqlOptionsAction: o => o.MigrationsAssembly("User"));

            return new UserContext(optionsBuilder.Options);
        }
    }
}
