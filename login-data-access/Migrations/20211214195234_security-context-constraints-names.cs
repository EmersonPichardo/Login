using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextconstraintsnames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sesions_users_User_Id",
                table: "sesions");

            migrationBuilder.DropForeignKey(
                name: "FK_users_applications_Application_Id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_sesions_users",
                table: "sesions",
                column: "User_Id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_applications",
                table: "users",
                column: "Application_Id",
                principalTable: "applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sesions_users",
                table: "sesions");

            migrationBuilder.DropForeignKey(
                name: "fk_users_applications",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "FK_sesions_users_User_Id",
                table: "sesions",
                column: "User_Id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_applications_Application_Id",
                table: "users",
                column: "Application_Id",
                principalTable: "applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
