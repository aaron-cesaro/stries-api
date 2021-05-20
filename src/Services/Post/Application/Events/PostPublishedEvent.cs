using System;

namespace Post.Application.Events
{
    public class PostPublishedEvent
    {
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
    }
}