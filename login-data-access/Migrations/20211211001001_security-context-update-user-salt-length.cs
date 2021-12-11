using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextupdateusersaltlength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "users",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "users",
                type: "varbinary(8)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");
        }
    }
}
