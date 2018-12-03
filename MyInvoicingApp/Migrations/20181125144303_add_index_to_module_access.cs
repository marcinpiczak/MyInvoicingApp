using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class add_index_to_module_access : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Index",
                table: "UserModuleAccesses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Index",
                table: "RoleModuleAccesses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "UserModuleAccesses");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "RoleModuleAccesses");
        }
    }
}
