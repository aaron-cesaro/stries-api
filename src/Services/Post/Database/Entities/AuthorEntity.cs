using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Database.Entities
{
    [Table("Authors", Schema = "Post")]
    public class AuthorEntity
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }

        [StringLength(24, MinimumLength = 6)]
        public string NickName { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        [StringLength(280, MinimumLength = 22)]
        public string Biography { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Post Published field cannot be negative")]
        public int PostPublished { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
