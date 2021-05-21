using System;

namespace Post.Application.Events
{
    public class AuthorDeletedEvent
    {
        public Guid Id { get; set; }
    }
}
