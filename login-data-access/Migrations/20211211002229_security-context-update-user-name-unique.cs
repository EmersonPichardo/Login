using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextupdateusernameunique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ind_users_name",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "unq_users_name",
                table: "users",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "unq_users_name",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "ind_users_name",
                table: "users",
                column: "Name");
        }
    }
}
