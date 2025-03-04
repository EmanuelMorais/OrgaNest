using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrgaNestApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedAt",
                table: "Categories",
                type: "TEXT",
                nullable: true); // Make it temporarily nullable

// Manually set the CreatedAt value for existing records
            migrationBuilder.Sql("UPDATE Categories SET CreatedAt = strftime('%Y-%m-%d %H:%M:%f', 'now') WHERE CreatedAt IS NULL;");

// Alter the column to make it NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "CreatedAt",
                table: "Categories",
                type: "TEXT",
                nullable: false);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_CreatedAt_Id",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Categories");
        }
    }
}
