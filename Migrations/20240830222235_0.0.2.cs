using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HAI_Selenium.Migrations
{
    /// <inheritdoc />
    public partial class _002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDateRequests_InvoiceRequests_InvoiceRequestId",
                table: "ServiceDateRequests");

            migrationBuilder.DropTable(
                name: "InvoiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDateRequests_InvoiceRequestId",
                table: "ServiceDateRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateOfBirth = table.Column<string>(type: "text", nullable: false),
                    DiagnosisCodes = table.Column<List<string>>(type: "text[]", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    InvoiceId = table.Column<int>(type: "integer", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PolicyNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDateRequests_InvoiceRequestId",
                table: "ServiceDateRequests",
                column: "InvoiceRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDateRequests_InvoiceRequests_InvoiceRequestId",
                table: "ServiceDateRequests",
                column: "InvoiceRequestId",
                principalTable: "InvoiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
