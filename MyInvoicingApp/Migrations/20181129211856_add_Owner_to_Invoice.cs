using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class add_Owner_to_Invoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OwnerId",
                table: "Invoices",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AspNetUsers_OwnerId",
                table: "Invoices",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AspNetUsers_OwnerId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_OwnerId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Invoices");
        }
    }
}
