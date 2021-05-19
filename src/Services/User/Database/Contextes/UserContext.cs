﻿using Microsoft.EntityFrameworkCore;
using User.Database.Entities;

namespace User.Database.Contextes
{
    public class UserContext : DbContext
    {
        public DbSet<UsersInfoEntity> Users { get; set; }

        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

    }
}
