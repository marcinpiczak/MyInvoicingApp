using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class Invoice_modify_column_invoiced : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoicesAmount",
                table: "Budgets",
                newName: "InvoicedAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoicedAmount",
                table: "Budgets",
                newName: "InvoicesAmount");
        }
    }
}
