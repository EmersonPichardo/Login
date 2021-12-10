using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextconnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(75)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<byte[]>(type: "varbinary(20)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sesions",
                columns: table => new
                {
                    Token = table.Column<byte[]>(type: "varbinary(3072)", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ValidSince = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sesions", x => x.Token);
                    table.ForeignKey(
                        name: "FK_sesions_users_User_Id",
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

            migrationBuilder.CreateIndex(
                name: "ind_users_name",
                table: "users",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sesions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
