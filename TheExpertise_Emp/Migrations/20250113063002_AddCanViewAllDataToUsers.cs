using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheExpertise_Emp.Migrations
{
    /// <inheritdoc />
    public partial class AddCanViewAllDataToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanViewAllData",
                table: "AdminUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanViewAllData",
                table: "AdminUsers");
        }
    }
}
