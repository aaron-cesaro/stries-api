using Post.Api.Database.Models;
using System;

namespace Post.Api.Application.Models
{
    public class PostReponse
    {
        public Guid Id { get; set; }
        public AuthorResponse Author { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public PostStatus Status { get; set; }
        public PostData PostData { get; set; }
        public decimal Rating { get; set; }
        public int RatingVoters { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
