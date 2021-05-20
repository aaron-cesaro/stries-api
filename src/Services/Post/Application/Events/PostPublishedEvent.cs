using System;

namespace Post.Application.Events
{
    public class PostPublishedEvent
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}