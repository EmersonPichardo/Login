using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class SecurityDefaultSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "security");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "sesions",
                newName: "sesions",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "applications",
                newName: "applications",
                newSchema: "security");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "users",
                schema: "security",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "sesions",
                schema: "security",
                newName: "sesions");

            migrationBuilder.RenameTable(
                name: "applications",
                schema: "security",
                newName: "applications");
        }
    }
}
