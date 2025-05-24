using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(32)", nullable: false),
                    Name = table.Column<string>(type: "varchar(16)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Application_Id = table.Column<string>(type: "char(32)", nullable: false),
                    Email = table.Column<string>(type: "varchar(64)", nullable: false),
                    Name = table.Column<string>(type: "varchar(16)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.Id);
                    table.ForeignKey(
                        name: "fk_users_applications",
                        column: x => x.Application_Id,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sesions",
                columns: table => new
                {
                    Token = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
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
                });

            migrationBuilder.CreateIndex(
                name: "ind_applications_name",
                table: "applications",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "ind_sesions_user_id",
                table: "sesions",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "ind_users_application_id",
                table: "users",
                column: "Application_Id");

            migrationBuilder.CreateIndex(
                name: "unq_users_email",
                table: "users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sesions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "applications");
        }
    }
}
