using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class change_OwnerId_requirements_for_Invoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
