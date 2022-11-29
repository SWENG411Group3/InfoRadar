using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class FixTagRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lighthouses_Tags_TagId",
                table: "Lighthouses");

            migrationBuilder.DropIndex(
                name: "IX_Lighthouses_TagId",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Lighthouses");

            migrationBuilder.CreateTable(
                name: "LighthouseTag",
                columns: table => new
                {
                    LighthousesId = table.Column<int>(type: "int", nullable: false),
                    TagsTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LighthouseTag", x => new { x.LighthousesId, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_LighthouseTag_Lighthouses_LighthousesId",
                        column: x => x.LighthousesId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LighthouseTag_Tags_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LighthouseTag_TagsTagId",
                table: "LighthouseTag",
                column: "TagsTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LighthouseTag");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Lighthouses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lighthouses_TagId",
                table: "Lighthouses",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lighthouses_Tags_TagId",
                table: "Lighthouses",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");
        }
    }
}
