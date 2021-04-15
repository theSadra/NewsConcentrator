using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsConcentratorSystem.Migrations
{
    public partial class mustnotcontainmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageCutAfters_Channels_TelegramChannelChannelId",
                table: "MessageCutAfters");

            migrationBuilder.DropIndex(
                name: "IX_MessageCutAfters_TelegramChannelChannelId",
                table: "MessageCutAfters");

            migrationBuilder.DropColumn(
                name: "TelegramChannelChannelId",
                table: "MessageCutAfters");

            migrationBuilder.CreateTable(
                name: "MessageMustnotContain",
                columns: table => new
                {
                    MMCId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MustnotContainWord = table.Column<string>(type: "TEXT", nullable: false),
                    TelegramChannelChannelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageMustnotContain", x => x.MMCId);
                    table.ForeignKey(
                        name: "FK_MessageMustnotContain_Channels_TelegramChannelChannelId",
                        column: x => x.TelegramChannelChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageMustnotContain_TelegramChannelChannelId",
                table: "MessageMustnotContain",
                column: "TelegramChannelChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageMustnotContain");

            migrationBuilder.AddColumn<int>(
                name: "TelegramChannelChannelId",
                table: "MessageCutAfters",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deschatid = table.Column<string>(type: "TEXT", nullable: false),
                    Intervalmins = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageCutAfters_TelegramChannelChannelId",
                table: "MessageCutAfters",
                column: "TelegramChannelChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageCutAfters_Channels_TelegramChannelChannelId",
                table: "MessageCutAfters",
                column: "TelegramChannelChannelId",
                principalTable: "Channels",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
