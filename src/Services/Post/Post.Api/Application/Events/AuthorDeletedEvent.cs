using System;

namespace Post.Api.Application.Events
{
    public class AuthorDeletedEvent
    {
        public Guid Id { get; set; }
    }
}
