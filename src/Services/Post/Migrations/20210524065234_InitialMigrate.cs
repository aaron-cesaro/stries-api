using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Post.Database.Entities;

namespace Post.Migrations
{
    public partial class InitialMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Post");

            migrationBuilder.CreateTable(
                name: "Authors",
                schema: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NickName = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: true),
                    Biography = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: true),
                    PostPublished = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Summary = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(200)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<PostBody>(type: "jsonb", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric(1,1)", nullable: false),
                    RatingVoters = table.Column<decimal>(type: "numeric(1,1)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authors",
                schema: "Post");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "Post");
        }
    }
}
