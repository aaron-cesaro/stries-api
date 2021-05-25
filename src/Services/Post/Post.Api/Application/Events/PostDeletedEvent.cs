using System;

namespace Post.Api.Application.Events
{
    public class PostDeletedEvent
    {
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
    }
}