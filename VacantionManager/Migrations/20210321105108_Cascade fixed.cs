using Microsoft.EntityFrameworkCore.Migrations;

namespace VacantionManager.Migrations
{
    public partial class Cascadefixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Projects_projectid",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_teamLeaderId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_User_Team",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Projects_projectid",
                table: "Teams",
                column: "projectid",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_teamLeaderId",
                table: "Teams",
                column: "teamLeaderId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_User_Team",
                table: "Users",
                column: "teamid",
                principalTable: "Teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Projects_projectid",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_teamLeaderId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_User_Team",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Projects_projectid",
                table: "Teams",
                column: "projectid",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_teamLeaderId",
                table: "Teams",
                column: "teamLeaderId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_User_Team",
                table: "Users",
                column: "teamid",
                principalTable: "Teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
