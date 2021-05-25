using System;

namespace Post.Api.Application.Events
{
    public class PostArchivedEvent
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime ArchivedAt { get; set; }
    }
}