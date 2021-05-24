using System;

namespace Post.Api.Application.Events
{
    public class PostDeletedEvent
    {
        public Guid PostDeleted_PostId { get; set; }
        public Guid PostDeleted_AuthorId { get; set; }
    }
}