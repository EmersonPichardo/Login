using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextuserspasswordlength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Password",
                table: "users",
                type: "varbinary(128)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(64)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Password",
                table: "users",
                type: "varbinary(64)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(128)");
        }
    }
}
