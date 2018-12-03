using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class add_column_details_for_module_access : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Details",
                table: "ModuleAccesses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "ModuleAccesses");
        }
    }
}
