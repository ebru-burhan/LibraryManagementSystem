using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMembershipAndUserRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "MembershipApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MembershipTypeId",
                table: "MembershipApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MembershipApplicationId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MembershipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ExternalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipApplications_MembershipTypeId",
                table: "MembershipApplications",
                column: "MembershipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MembershipApplicationId",
                table: "Members",
                column: "MembershipApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipTypes_Code",
                table: "MembershipTypes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MembershipApplications_MembershipApplicationId",
                table: "Members",
                column: "MembershipApplicationId",
                principalTable: "MembershipApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipApplications_MembershipTypes_MembershipTypeId",
                table: "MembershipApplications",
                column: "MembershipTypeId",
                principalTable: "MembershipTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MembershipApplications_MembershipApplicationId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipApplications_MembershipTypes_MembershipTypeId",
                table: "MembershipApplications");

            migrationBuilder.DropTable(
                name: "MembershipTypes");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_MembershipApplications_MembershipTypeId",
                table: "MembershipApplications");

            migrationBuilder.DropIndex(
                name: "IX_Members_MembershipApplicationId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "MembershipTypeId",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "MembershipApplicationId",
                table: "Members");
        }
    }
}
