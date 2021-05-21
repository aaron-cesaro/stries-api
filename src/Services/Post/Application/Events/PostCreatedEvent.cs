using System;
using System.ComponentModel.DataAnnotations;

namespace Post.Application.EventHandlers.Events
{
    public class PostCreatedEvent
    {
        [Required(ErrorMessage = "Author Id field must be provided")]
        public Guid PostCreated_AuthorId { get; set; }

        [Required(ErrorMessage = "Title field must be provided")]
        [StringLength(60, MinimumLength = 6)]
        public string PostCreated_Title { get; set; }

        [Required(ErrorMessage = "Url field must be provided")]
        public string PostCreated_ImageUrl { get; set; }

        [Required(ErrorMessage = "Created at field must be provided")]
        public DateTime PostCreated_CreatedAt { get; set; }
    }
}
