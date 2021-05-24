using System;
using System.ComponentModel.DataAnnotations;

namespace Post.Api.Application.Models
{
    public class AuthorResponse
    {
        [Required(ErrorMessage = "Id field must be provided")]
        public Guid Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "First Name field must be provided")]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Last Name field must be provided")]
        public string LastName { get; set; }

        [StringLength(24, MinimumLength = 6)]
        [Required(ErrorMessage = "Nickname field must be provided")]
        public string NickName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email address field must be provided")]
        public string EmailAddress { get; set; }

        [StringLength(280, MinimumLength = 22)]
        public string Biography { get; set; }

        [Required(ErrorMessage = "Created At field must be provided")]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
