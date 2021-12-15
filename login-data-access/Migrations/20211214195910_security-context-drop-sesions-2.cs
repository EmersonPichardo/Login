using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextdropsesions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sesions");

            migrationBuilder.RenameIndex(
                name: "IX_users_Application_Id",
                table: "users",
                newName: "ind_users_application_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ind_users_application_id",
                table: "users",
                newName: "IX_users_Application_Id");

            migrationBuilder.CreateTable(
                name: "sesions",
                columns: table => new
                {
                    Token = table.Column<byte[]>(type: "varbinary(16)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sesions", x => x.Token);
                    table.ForeignKey(
                        name: "fk_sesions_users",
                        column: x => x.User_Id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_sesions_User_Id",
                table: "sesions",
                column: "User_Id");
        }
    }
}
