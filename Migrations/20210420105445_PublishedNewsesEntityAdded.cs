using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsConcentratorSystem.Migrations
{
    public partial class PublishedNewsesEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishedNewseses",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mediahash = table.Column<string>(type: "TEXT", nullable: true),
                    TextMessage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedNewseses", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedNewseses");
        }
    }
}
