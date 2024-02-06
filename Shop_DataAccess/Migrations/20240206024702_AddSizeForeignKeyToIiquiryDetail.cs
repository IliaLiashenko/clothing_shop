using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeForeignKeyToIiquiryDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetail_SizeId",
                table: "InquiryDetail",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_Size_SizeId",
                table: "InquiryDetail",
                column: "SizeId",
                principalTable: "Size",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_Size_SizeId",
                table: "InquiryDetail");

            migrationBuilder.DropIndex(
                name: "IX_InquiryDetail_SizeId",
                table: "InquiryDetail");
        }
    }
}
