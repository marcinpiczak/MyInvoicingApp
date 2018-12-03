using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class change_structure_for_module_Access : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleAccesses");

            migrationBuilder.CreateTable(
                name: "RoleModuleAccesses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedById = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AccessorType = table.Column<int>(nullable: false),
                    AccessorId = table.Column<string>(maxLength: 450, nullable: false),
                    Module = table.Column<int>(nullable: false),
                    Add = table.Column<bool>(nullable: false),
                    Edit = table.Column<bool>(nullable: false),
                    Close = table.Column<bool>(nullable: false),
                    Open = table.Column<bool>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Send = table.Column<bool>(nullable: false),
                    Details = table.Column<bool>(nullable: false),
                    Approve = table.Column<bool>(nullable: false),
                    Remove = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleModuleAccesses", x => new { x.AccessorId, x.Module });
                    table.UniqueConstraint("AK_RoleModuleAccesses_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleModuleAccesses_AspNetRoles_AccessorId",
                        column: x => x.AccessorId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleModuleAccesses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleModuleAccesses_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserModuleAccesses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedById = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AccessorType = table.Column<int>(nullable: false),
                    AccessorId = table.Column<string>(maxLength: 450, nullable: false),
                    Module = table.Column<int>(nullable: false),
                    Add = table.Column<bool>(nullable: false),
                    Edit = table.Column<bool>(nullable: false),
                    Close = table.Column<bool>(nullable: false),
                    Open = table.Column<bool>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Send = table.Column<bool>(nullable: false),
                    Details = table.Column<bool>(nullable: false),
                    Approve = table.Column<bool>(nullable: false),
                    Remove = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModuleAccesses", x => new { x.AccessorId, x.Module });
                    table.UniqueConstraint("AK_UserModuleAccesses_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserModuleAccesses_AspNetUsers_AccessorId",
                        column: x => x.AccessorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserModuleAccesses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserModuleAccesses_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleModuleAccesses_CreatedById",
                table: "RoleModuleAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoleModuleAccesses_LastModifiedById",
                table: "RoleModuleAccesses",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAccesses_CreatedById",
                table: "UserModuleAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleAccesses_LastModifiedById",
                table: "UserModuleAccesses",
                column: "LastModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleModuleAccesses");

            migrationBuilder.DropTable(
                name: "UserModuleAccesses");

            migrationBuilder.CreateTable(
                name: "ModuleAccesses",
                columns: table => new
                {
                    RoleId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Module = table.Column<int>(nullable: false),
                    Add = table.Column<bool>(nullable: false),
                    Approve = table.Column<bool>(nullable: false),
                    Cancel = table.Column<bool>(nullable: false),
                    Close = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Details = table.Column<bool>(nullable: false),
                    Edit = table.Column<bool>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    LastModifiedById = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Open = table.Column<bool>(nullable: false),
                    Remove = table.Column<bool>(nullable: false),
                    Send = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleAccesses", x => new { x.RoleId, x.UserId, x.Module });
                    table.UniqueConstraint("AK_ModuleAccesses_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleAccesses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_ModuleAccesses_UserId",
                table: "ModuleAccesses",
                column: "UserId");
        }
    }
}
