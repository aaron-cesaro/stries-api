using System;
using System.ComponentModel.DataAnnotations;

namespace Post.Application.EventHandlers.Events
{
    public class PostCreatedEvent
    {
        [Required(ErrorMessage = "Author Id field must be provided")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "Title field must be provided")]
        [StringLength(60, MinimumLength = 6)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Url field must be provided")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Created at field must be provided")]
        public DateTime CreatedAt { get; set; }
    }
}
