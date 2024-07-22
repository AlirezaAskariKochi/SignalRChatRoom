using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalRChatRoom.Server.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Chats");

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersianName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupType = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: true),
                    EntityId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                schema: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromId = table.Column<long>(type: "bigint", nullable: false),
                    ToId = table.Column<long>(type: "bigint", nullable: true),
                    GroupId = table.Column<long>(type: "bigint", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReplyId = table.Column<long>(type: "bigint", nullable: true),
                    ForwardId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Clients_FromId",
                        column: x => x.FromId,
                        principalSchema: "Chats",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Clients_ToId",
                        column: x => x.ToId,
                        principalSchema: "Chats",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Chats",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupClients",
                schema: "Chats",
                columns: table => new
                {
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupClients", x => new { x.ClientId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_GroupClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Chats",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupClients_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Chats",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeenMessageLog",
                schema: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatRoomId = table.Column<long>(type: "bigint", nullable: false),
                    SeenDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeenMessageLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeenMessageLog_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalSchema: "Chats",
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_FromId",
                schema: "Chats",
                table: "ChatRooms",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_GroupId",
                schema: "Chats",
                table: "ChatRooms",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_ToId",
                schema: "Chats",
                table: "ChatRooms",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupClients_GroupId",
                schema: "Chats",
                table: "GroupClients",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SeenMessageLog_ChatRoomId",
                schema: "Chats",
                table: "SeenMessageLog",
                column: "ChatRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupClients",
                schema: "Chats");

            migrationBuilder.DropTable(
                name: "SeenMessageLog",
                schema: "Chats");

            migrationBuilder.DropTable(
                name: "ChatRooms",
                schema: "Chats");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "Chats");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "Chats");
        }
    }
}
