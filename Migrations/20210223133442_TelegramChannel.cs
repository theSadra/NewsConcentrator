using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsConcentratorSystem.Migrations
{
    public partial class TelegramChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityStatus",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "HasContainFilter",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "IntervalMins",
                table: "Channels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ActivityStatus",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasContainFilter",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IntervalMins",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
