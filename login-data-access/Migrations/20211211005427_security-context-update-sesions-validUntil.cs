using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextupdatesesionsvalidUntil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidSince",
                table: "sesions",
                newName: "ValidUntil");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                table: "sesions",
                newName: "ValidSince");
        }
    }
}
