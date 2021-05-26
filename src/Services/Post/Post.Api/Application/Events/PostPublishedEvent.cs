using System;
using System.ComponentModel.DataAnnotations;

namespace Post.Api.Application.Events
{
    public class PostPublishedEvent
    {
        [Required(ErrorMessage = "Post Id field must be provided")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Author Id field must be provided")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "Title field must be provided")]
        [StringLength(60, MinimumLength = 6)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Title field must be provided")]
        [StringLength(280, MinimumLength = 28)]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Url field must be provided")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Published at field must be provided")]
        public DateTime PublishedAt { get; set; }
    }
}