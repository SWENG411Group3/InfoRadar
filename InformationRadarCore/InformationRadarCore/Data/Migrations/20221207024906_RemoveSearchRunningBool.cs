using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class RemoveSearchRunningBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchRunning",
                table: "Lighthouses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SearchRunning",
                table: "Lighthouses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
