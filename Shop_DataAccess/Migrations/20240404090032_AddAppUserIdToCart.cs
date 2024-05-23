using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserIdToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ShoppingCart",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_ApplicationUserId",
                table: "ShoppingCart",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_ApplicationUserId",
                table: "ShoppingCart",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_ApplicationUserId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_ApplicationUserId",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ShoppingCart");
        }
    }
}
