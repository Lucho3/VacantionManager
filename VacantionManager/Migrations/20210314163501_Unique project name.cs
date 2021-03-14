using Microsoft.EntityFrameworkCore.Migrations;

namespace VacantionManager.Migrations
{
    public partial class Uniqueprojectname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_name",
                table: "Projects",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_name",
                table: "Projects");
        }
    }
}
