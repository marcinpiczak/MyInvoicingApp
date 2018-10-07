using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class Invoice_add_change_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Budget",
                table: "Invoices",
                newName: "BudgetId");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Invoices",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BudgetId",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveDate",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BudgetId",
                table: "Invoices",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Budgets_BudgetId",
                table: "Invoices",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Budgets_BudgetId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BudgetId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ReceiveDate",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "Invoices",
                newName: "Budget");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Invoices",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Budget",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
