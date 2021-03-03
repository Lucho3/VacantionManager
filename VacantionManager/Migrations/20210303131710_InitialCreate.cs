using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VacantionManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    projectid = table.Column<int>(type: "int", nullable: true),
                    teamLeaderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_Teams_Projects_projectid",
                        column: x => x.projectid,
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    firstName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    roleid = table.Column<int>(type: "int", nullable: false),
                    teamid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_roleid",
                        column: x => x.roleid,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_User_Team",
                        column: x => x.teamid,
                        principalTable: "Teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HospitalLeaves",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    startDate = table.Column<DateTime>(type: "date", nullable: false),
                    endDate = table.Column<DateTime>(type: "date", nullable: false),
                    appicationDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "getdate()"),
                    approved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ambulatoryCard = table.Column<byte[]>(type: "image", nullable: false),
                    applicantid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalLeaves", x => x.id);
                    table.ForeignKey(
                        name: "FK_HospitalLeaves_Users_applicantid",
                        column: x => x.applicantid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leaves",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isPaid = table.Column<bool>(type: "bit", nullable: false),
                    startDate = table.Column<DateTime>(type: "date", nullable: false),
                    endDate = table.Column<DateTime>(type: "date", nullable: false),
                    appicationDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "getdate()"),
                    halfDay = table.Column<bool>(type: "bit", nullable: false),
                    approved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    applicantid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaves", x => x.id);
                    table.ForeignKey(
                        name: "FK_Leaves_Users_applicantid",
                        column: x => x.applicantid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HospitalLeaves_applicantid",
                table: "HospitalLeaves",
                column: "applicantid");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_applicantid",
                table: "Leaves",
                column: "applicantid");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_projectid",
                table: "Teams",
                column: "projectid");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_teamLeaderid",
                table: "Teams",
                column: "teamLeaderid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_roleid",
                table: "Users",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_teamid",
                table: "Users",
                column: "teamid");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Team_Leader",
                table: "Teams",
                column: "teamLeaderid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Team_Leader",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "HospitalLeaves");

            migrationBuilder.DropTable(
                name: "Leaves");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
