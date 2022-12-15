using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class MoreUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Templates",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Templates",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Lighthouses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SearchRunning",
                table: "Lighthouses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "SearchRunning",
                table: "Lighthouses");
        }
    }
}
