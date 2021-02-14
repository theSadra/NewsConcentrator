using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsConcentratorSystem.Migrations
{
    public partial class initmig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ChannelId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelUserName = table.Column<string>(type: "TEXT", nullable: false),
                    HasContainFilter = table.Column<bool>(type: "INTEGER", nullable: false),
                    IntervalMins = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivityStatus = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "MessageCutAfters",
                columns: table => new
                {
                    MCAId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CutAfterWord = table.Column<string>(type: "TEXT", nullable: true),
                    TelegramChannelChannelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCutAfters", x => x.MCAId);
                    table.ForeignKey(
                        name: "FK_MessageCutAfters_Channels_TelegramChannelChannelId",
                        column: x => x.TelegramChannelChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageMustContains",
                columns: table => new
                {
                    MMCId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MustContainWord = table.Column<string>(type: "TEXT", nullable: false),
                    TelegramChannelChannelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageMustContains", x => x.MMCId);
                    table.ForeignKey(
                        name: "FK_MessageMustContains_Channels_TelegramChannelChannelId",
                        column: x => x.TelegramChannelChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageReplaceWords",
                columns: table => new
                {
                    MRWId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Word = table.Column<string>(type: "TEXT", nullable: false),
                    ReplaceTo = table.Column<string>(type: "TEXT", nullable: false),
                    TelegramChannelChannelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReplaceWords", x => x.MRWId);
                    table.ForeignKey(
                        name: "FK_MessageReplaceWords_Channels_TelegramChannelChannelId",
                        column: x => x.TelegramChannelChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageCutAfters_TelegramChannelChannelId",
                table: "MessageCutAfters",
                column: "TelegramChannelChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageMustContains_TelegramChannelChannelId",
                table: "MessageMustContains",
                column: "TelegramChannelChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReplaceWords_TelegramChannelChannelId",
                table: "MessageReplaceWords",
                column: "TelegramChannelChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageCutAfters");

            migrationBuilder.DropTable(
                name: "MessageMustContains");

            migrationBuilder.DropTable(
                name: "MessageReplaceWords");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
