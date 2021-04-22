using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsConcentratorSystem.Migrations
{
    public partial class PublishedNewsesEntityAdded : Migration
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
                    ChannelTitle = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelChatID = table.Column<string>(type: "TEXT", nullable: true),
                    AccessHash = table.Column<string>(type: "TEXT", nullable: true)
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
                    CutAfterWord = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCutAfters", x => x.MCAId);
                });

            migrationBuilder.CreateTable(
                name: "PublishedNewses",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mediahash = table.Column<string>(type: "TEXT", nullable: true),
                    TextMessage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedNewses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDescription = table.Column<string>(type: "TEXT", nullable: true),
                    EndDescription = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.id);
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
                name: "IX_MessageMustContains_TelegramChannelChannelId",
                table: "MessageMustContains",
                column: "TelegramChannelChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageMustnotContain_TelegramChannelChannelId",
                table: "MessageMustnotContain",
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
                name: "MessageMustnotContain");

            migrationBuilder.DropTable(
                name: "MessageReplaceWords");

            migrationBuilder.DropTable(
                name: "PublishedNewses");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
