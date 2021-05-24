using Microsoft.EntityFrameworkCore;
using User.Api.Database.Entities;

namespace User.Api.Database.Contextes
{
    public class UserContext : DbContext
    {
        public DbSet<UserInfoEntity> Users { get; set; }

        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

    }
}
