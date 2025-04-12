using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LU2_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorldSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxX",
                table: "Environments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxY",
                table: "Environments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxX",
                table: "Environments");

            migrationBuilder.DropColumn(
                name: "MaxY",
                table: "Environments");
        }
    }
}
