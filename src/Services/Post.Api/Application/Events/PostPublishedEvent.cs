using System;

namespace Post.Api.Application.Events
{
    public class PostPublishedEvent
    {
        public Guid PostPublished_PostId { get; set; }
        public string PostPublished_Title { get; set; }
        public string PostPublished_Summary { get; set; }
        public string PostPublished_ImageUrl { get; set; }
        public Guid PostPublished_AuthorId { get; set; }
        public DateTime PostPublished_PublishedAt { get; set; }
    }
}