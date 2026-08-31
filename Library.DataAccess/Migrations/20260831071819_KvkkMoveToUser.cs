using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class KvkkMoveToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKvkkApproved",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "IsTermsAccepted",
                table: "MembershipApplications");

            migrationBuilder.AddColumn<bool>(
                name: "IsKvkkApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTermsAccepted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKvkkApproved",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsTermsAccepted",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsKvkkApproved",
                table: "MembershipApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTermsAccepted",
                table: "MembershipApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
