using Microsoft.EntityFrameworkCore.Migrations;

namespace VacantionManager.Migrations
{
    public partial class Cascadetest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_roleid",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_roleid",
                table: "Users",
                column: "roleid",
                principalTable: "Roles",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_roleid",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_roleid",
                table: "Users",
                column: "roleid",
                principalTable: "Roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
