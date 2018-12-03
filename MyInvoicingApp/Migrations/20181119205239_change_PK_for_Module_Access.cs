using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class change_PK_for_Module_Access : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleAccesses_AspNetUsers_CreatedById",
                table: "ModuleAccesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleAccesses",
                table: "ModuleAccesses");

            migrationBuilder.DropIndex(
                name: "IX_ModuleAccesses_RoleId",
                table: "ModuleAccesses");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModuleAccesses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ModuleAccesses_Id",
                table: "ModuleAccesses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleAccesses",
                table: "ModuleAccesses",
                columns: new[] { "RoleId", "UserId", "Module" });

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleAccesses_AspNetUsers_CreatedById",
                table: "ModuleAccesses",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleAccesses_AspNetUsers_CreatedById",
                table: "ModuleAccesses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ModuleAccesses_Id",
                table: "ModuleAccesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleAccesses",
                table: "ModuleAccesses");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ModuleAccesses",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleAccesses",
                table: "ModuleAccesses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleAccesses_RoleId",
                table: "ModuleAccesses",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleAccesses_AspNetUsers_CreatedById",
                table: "ModuleAccesses",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
