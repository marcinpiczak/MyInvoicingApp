using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class attachment_table_add_contentType_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Attachments",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Attachments");
        }
    }
}
