using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImageToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProfileImage",
                table: "UserTable",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.UpdateData(
                table: "UserTable",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImage",
                value: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "UserTable");
        }
    }
}
