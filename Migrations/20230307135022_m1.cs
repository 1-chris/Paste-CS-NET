using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paste_CS_NET.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pastes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Expiry = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pastes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Pastes",
                columns: new[] { "Id", "Content", "Expiry", "Language" },
                values: new object[] { new Guid("de274b65-cb7d-4668-9eee-5a32df269d52"), "Dummy paste", new DateTime(2123, 3, 7, 13, 50, 21, 970, DateTimeKind.Unspecified).AddTicks(2927), "Plaintext" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pastes");
        }
    }
}
