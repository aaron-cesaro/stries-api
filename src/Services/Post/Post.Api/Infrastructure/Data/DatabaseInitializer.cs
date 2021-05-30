using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Post.Api.Database.Contextes;
using Post.Api.Infrastructure.Data.Seeds;
using System.Linq;

namespace Post.Api.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        public static void DatabaseSeed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<PostContext>();

            context.Database.Migrate();

            // Clean post context tabels before insert new seeds
            CleanDatabaseSeed(context);

            foreach (var author in AuthorSeed.AuthorSeeds)
            {
                context.Add(author);
            }

            foreach (var post in PostSeed.PostSeeds)
            {
                context.Add(post);
            }

            context.SaveChanges();
        }

        private static void CleanDatabaseSeed(PostContext context)
        {
            var postsToDelete = context.Posts
                .Where(p => p.Id != System.Guid.Empty)
                .ToList();
            var authorsToDelete = context.Authors
                .Where(a => a.Id != System.Guid.Empty)
                .ToList(); ;

            context.RemoveRange(postsToDelete);
            context.RemoveRange(authorsToDelete);

            context.SaveChanges();
        }
    }
}
