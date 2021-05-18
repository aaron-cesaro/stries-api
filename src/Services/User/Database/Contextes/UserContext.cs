using Microsoft.EntityFrameworkCore;
using User.Database.Entities;

namespace User.Database.Contextes
{
    public class UserContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

    }
}
