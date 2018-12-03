using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class add_module_access : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleAccesses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedById = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Module = table.Column<int>(nullable: false),
                    Add = table.Column<bool>(nullable: false),
                    Edit = table.Column<bool>(nullable: false),
                    Close = table.Column<bool>(nullable: false),
                    Open = table.Column<bool>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Send = table.Column<bool>(nullable: false),
                    Approve = table.Column<bool>(nullable: false),
                    Remove = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleAccesses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleAccesses_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModuleAccesses_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleAccesses_CreatedById",
                table: "ModuleAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleAccesses_LastModifiedById",
                table: "ModuleAccesses",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleAccesses_RoleId",
                table: "ModuleAccesses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleAccesses_UserId",
                table: "ModuleAccesses",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleAccesses");
        }
    }
}
