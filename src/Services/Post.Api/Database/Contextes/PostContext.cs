using Microsoft.EntityFrameworkCore;
using Post.Api.Database.Entities;

namespace Post.Api.Database.Contextes
{
    public class PostContext : DbContext
    {
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<AuthorEntity> Authors { get; set; }

        public PostContext()
        {
        }

        public PostContext(DbContextOptions<PostContext> options)
            : base(options)
        {
        }
    }
}