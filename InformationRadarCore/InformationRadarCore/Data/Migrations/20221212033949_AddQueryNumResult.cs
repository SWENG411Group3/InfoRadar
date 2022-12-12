using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class AddQueryNumResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumResults",
                table: "GoogleQueries",
                type: "int",
                nullable: false,
                defaultValue: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumResults",
                table: "GoogleQueries");
        }
    }
}
