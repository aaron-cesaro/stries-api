using Post.Api.Application.Models;
using Post.Api.Database.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Api.Database.Entities
{
    /// <summary>
    /// Stries Post database representation
    /// </summary>
    [Table("Posts", Schema = "Post")]
    public class PostEntity
    {
        [Key]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Author Id field must be provided")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "Title field must be provided")]
        [StringLength(60, MinimumLength = 6)]
        public string Title { get; set; }

        [StringLength(280, MinimumLength = 28)]
        public string Summary { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Status field must be provided")]
        public PostStatus Status { get; set; }

        [Column(TypeName = "jsonb")]
        public PostBody Body { get; set; }

        [Column(TypeName = "decimal(1, 1)")]
        public decimal Rating { get; set; }

        [Column(TypeName = "decimal(1, 1)")]
        [Range(0, int.MaxValue, ErrorMessage = "Rating voters field cannot be negative")]
        public int RatingVoters { get; set; }

        public DateTime PublishedAt { get; set; }

        public DateTime ArchivedAt { get; set; }

        [Required(ErrorMessage = "Created at field must be provided")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Updated at field must be provided")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Representation of a json Post content. 
    /// The class contains all fields needed to fill up a Post with user generated contents.
    /// </summary>
    public class PostBody
    {
        public CompanyDescription CompanyDescription { get; set; }
        public CompanyThesis CompanyThesis { get; set; }
        public CompanyValuation CompanyValuation { get; set; }
        public CompanyFinancials CompanyFinancials { get; set; }
        public CompanyOther CompanyOther { get; set; }
    }
}