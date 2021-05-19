using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Database.Entities
{
    [Table("UsersInfo", Schema = "User")]
    public class UserInfoEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }

        [Required]
        [StringLength(24, MinimumLength = 6)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [StringLength(280, MinimumLength = 22)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
