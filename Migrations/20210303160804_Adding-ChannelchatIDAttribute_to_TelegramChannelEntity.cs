using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsConcentratorSystem.Migrations
{
    public partial class AddingChannelchatIDAttribute_to_TelegramChannelEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelChatID",
                table: "Channels",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelChatID",
                table: "Channels");
        }
    }
}
