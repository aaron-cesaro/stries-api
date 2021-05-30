using Post.Api.Database.Entities;
using System;
using System.Collections.Generic;

namespace Post.Api.Infrastructure.Data.Seeds
{
    public static class AuthorSeed
    {
        public static List<AuthorEntity> AuthorSeeds = new List<AuthorEntity>
        {
            new AuthorEntity
            {
                Id = Guid.Parse("3aa71d25-33b7-47fc-b59f-5089693d6603"),
                FirstName = "Aaron",
                LastName = "Cesaro",
                NickName = "nan01",
                EmailAddress = "fakeAuthor@mail.com",
                Biography = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent pellentesque urna quis elit eleifend cursus. Donec euismod magna quam, quis malesuada mi dictum a",
                PostPublished = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AuthorEntity
            {
                Id = Guid.Parse("638c857c-1e53-4dc3-9281-829b8e438904"),
                FirstName = "Aaron",
                LastName = "Cesaro",
                NickName = "nan01",
                EmailAddress = "fakeAuthor@mail.com",
                Biography = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent pellentesque urna quis elit eleifend cursus. Donec euismod magna quam, quis malesuada mi dictum a",
                PostPublished = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }
}