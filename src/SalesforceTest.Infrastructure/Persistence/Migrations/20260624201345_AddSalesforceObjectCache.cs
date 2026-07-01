using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceTest.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesforceObjectCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesforceObjectCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApiName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LabelPlural = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldsJson = table.Column<string>(type: "TEXT", nullable: false),
                    LastScannedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesforceObjectCaches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesforceObjectCaches_UserId_ApiName",
                table: "SalesforceObjectCaches",
                columns: new[] { "UserId", "ApiName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesforceObjectCaches");
        }
    }
}
