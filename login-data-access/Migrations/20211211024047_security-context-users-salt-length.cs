using Microsoft.EntityFrameworkCore.Migrations;

namespace login_data_access.Migrations
{
    public partial class securitycontextuserssaltlength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "users",
                type: "varbinary(32)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(16)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "users",
                type: "varbinary(16)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)");
        }
    }
}
