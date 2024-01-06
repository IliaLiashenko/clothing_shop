using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace clothing_shop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNameColToSizeandToColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Size",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Size");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Colors");
        }
    }
}
