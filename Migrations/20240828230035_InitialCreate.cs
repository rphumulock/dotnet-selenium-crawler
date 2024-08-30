using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HAI_Selenium.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PolicyNumber = table.Column<string>(type: "text", nullable: false),
                    DiagnosisCodes = table.Column<List<string>>(type: "text[]", nullable: false),
                    DateOfBirth = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDateRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceDate = table.Column<string>(type: "text", nullable: false),
                    Counselor = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<string>(type: "text", nullable: false),
                    Other = table.Column<string>(type: "text", nullable: false),
                    TreatmentType = table.Column<string>(type: "text", nullable: false),
                    InvoiceRequestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDateRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDateRequests_InvoiceRequests_InvoiceRequestId",
                        column: x => x.InvoiceRequestId,
                        principalTable: "InvoiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDateRequests_InvoiceRequestId",
                table: "ServiceDateRequests",
                column: "InvoiceRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceDateRequests");

            migrationBuilder.DropTable(
                name: "InvoiceRequests");
        }
    }
}
