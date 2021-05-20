using System;
using System.ComponentModel.DataAnnotations;

namespace Post.Application.Models
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "Author Id field must be provided")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "Title field must be provided")]
        [StringLength(60, MinimumLength = 6)]
        public string Title { get; set; }

        [StringLength(280, MinimumLength = 28)]
        [Required(ErrorMessage = "Summary field must be provided")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Url field must be provided")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Post Data field must be provided")]
        public PostData PostData { get; set; }
    }
}