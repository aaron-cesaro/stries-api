using System;
using System.ComponentModel.DataAnnotations;

namespace Post.Database.Models
{
    public class TextDescription
    {
        [Required(ErrorMessage = "Text field must be provided")]
        [StringLength(10000, MinimumLength = 1)]
        public string Text { get; set; }

        [Required(ErrorMessage = "Created at field must be provided")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Updated at field must be provided")]
        public DateTime UpdatedAt { get; set; }
    }
}
