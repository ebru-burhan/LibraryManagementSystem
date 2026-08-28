using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMembershipAndUserEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "MembershipApplications");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MembershipApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityNumber",
                table: "Users",
                column: "IdentityNumber",
                unique: true,
                filter: "[IdentityNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipApplications_UserId",
                table: "MembershipApplications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipApplications_Users_UserId",
                table: "MembershipApplications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipApplications_Users_UserId",
                table: "MembershipApplications");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityNumber",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_MembershipApplications_UserId",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MembershipApplications");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "MembershipApplications",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "MembershipApplications",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "MembershipApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityNumber",
                table: "Users",
                column: "IdentityNumber",
                unique: true);
        }
    }
}
