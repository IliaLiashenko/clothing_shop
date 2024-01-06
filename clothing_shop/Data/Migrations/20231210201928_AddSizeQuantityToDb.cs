using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace clothing_shop.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeQuantityToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "ProductSizes");
        }
    }
}
