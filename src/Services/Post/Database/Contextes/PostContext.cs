using Microsoft.EntityFrameworkCore;
using Post.Database.Entities;

namespace Post.Database.Contextes
{
    public class PostContext : DbContext
    {
        public DbSet<PostEntity> Posts { get; set; }

        public PostContext(DbContextOptions<PostContext> options)
            : base(options)
        {
        }
    }
}