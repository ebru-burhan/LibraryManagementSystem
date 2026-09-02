using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MemberManipulationWAIAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Members");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MemberStatuses",
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
                    table.PrimaryKey("PK_MemberStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_StatusId",
                table: "Members",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberStatuses_Code",
                table: "MemberStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MemberStatuses_StatusId",
                table: "Members",
                column: "StatusId",
                principalTable: "MemberStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MemberStatuses_StatusId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "MemberStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Members_StatusId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Members");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
