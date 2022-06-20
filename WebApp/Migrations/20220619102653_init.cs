using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    displayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profilePic = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userName);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Identifier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    server = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastdate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_Contact_User_userName",
                        column: x => x.userName,
                        principalTable: "User",
                        principalColumn: "userName");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sent = table.Column<bool>(type: "bit", nullable: false),
                    ContactIdentifier = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.id);
                    table.ForeignKey(
                        name: "FK_Message_Contact_ContactIdentifier",
                        column: x => x.ContactIdentifier,
                        principalTable: "Contact",
                        principalColumn: "Identifier");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contact_userName",
                table: "Contact",
                column: "userName");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ContactIdentifier",
                table: "Message",
                column: "ContactIdentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
