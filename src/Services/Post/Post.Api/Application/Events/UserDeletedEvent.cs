using System;

namespace Post.Api.Application.Events
{
    public class UserDeletedEvent
    {
        public Guid Id { get; set; }
    }
}
