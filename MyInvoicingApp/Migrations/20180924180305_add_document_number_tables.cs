using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyInvoicingApp.Migrations
{
    public partial class add_document_number_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Invoices",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentSequences",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedById = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    StartNumber = table.Column<int>(nullable: false),
                    NextNumber = table.Column<int>(nullable: false),
                    MaxNumber = table.Column<int>(nullable: false),
                    EffectiveFrom = table.Column<DateTime>(nullable: false),
                    EffectiveTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSequences_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSequences_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentNumbers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedById = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedById = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    DocumentType = table.Column<int>(nullable: false),
                    DocumentSequenceId = table.Column<string>(nullable: false),
                    DocumentNumberPart1 = table.Column<string>(maxLength: 5, nullable: false),
                    DocumentNumberPart1Separator = table.Column<string>(maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentNumbers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentNumbers_DocumentSequences_DocumentSequenceId",
                        column: x => x.DocumentSequenceId,
                        principalTable: "DocumentSequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentNumbers_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumbers_CreatedById",
                table: "DocumentNumbers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumbers_DocumentSequenceId",
                table: "DocumentNumbers",
                column: "DocumentSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumbers_LastModifiedById",
                table: "DocumentNumbers",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSequences_CreatedById",
                table: "DocumentSequences",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSequences_LastModifiedById",
                table: "DocumentSequences",
                column: "LastModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentNumbers");

            migrationBuilder.DropTable(
                name: "DocumentSequences");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Invoices");
        }
    }
}
