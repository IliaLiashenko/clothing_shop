using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTotalToInquiryHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OrderTotal",
                table: "InquiryHeader",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderTotal",
                table: "InquiryHeader");
        }
    }
}
