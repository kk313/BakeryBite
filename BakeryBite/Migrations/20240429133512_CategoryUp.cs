using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BakeryBite.Migrations
{
    /// <inheritdoc />
    public partial class CategoryUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageBack",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageTitle",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBack",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ImageTitle",
                table: "Category");
        }
    }
}
