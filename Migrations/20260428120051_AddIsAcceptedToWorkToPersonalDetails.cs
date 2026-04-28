using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace student_profile.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAcceptedToWorkToPersonalDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedToWork",
                table: "PersonalDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptedToWork",
                table: "PersonalDetails");
        }
    }
}
