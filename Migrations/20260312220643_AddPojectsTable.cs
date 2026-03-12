using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace student_profile.Migrations
{
    /// <inheritdoc />
    public partial class AddPojectsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinkGitHub",
                table: "Projects",
                newName: "GitHubLink");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GitHubLink",
                table: "Projects",
                newName: "LinkGitHub");
        }
    }
}
