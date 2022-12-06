using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class DbUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoogleQueryLighthouse");

            migrationBuilder.DropTable(
                name: "LighthouseSite");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Sites",
                newName: "LighthouseId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lighthouses",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "HasError",
                table: "Lighthouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LatestLog",
                table: "Lighthouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Running",
                table: "Lighthouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Lighthouses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Lighthouses",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LighthouseId",
                table: "GoogleQueries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LighthouseId",
                table: "Sites",
                column: "LighthouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Lighthouses_TagId",
                table: "Lighthouses",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_GoogleQueries_LighthouseId",
                table: "GoogleQueries",
                column: "LighthouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagName",
                table: "Tags",
                column: "TagName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GoogleQueries_Lighthouses_LighthouseId",
                table: "GoogleQueries",
                column: "LighthouseId",
                principalTable: "Lighthouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lighthouses_Tags_TagId",
                table: "Lighthouses",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Lighthouses_LighthouseId",
                table: "Sites",
                column: "LighthouseId",
                principalTable: "Lighthouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoogleQueries_Lighthouses_LighthouseId",
                table: "GoogleQueries");

            migrationBuilder.DropForeignKey(
                name: "FK_Lighthouses_Tags_TagId",
                table: "Lighthouses");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Lighthouses_LighthouseId",
                table: "Sites");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Sites_LighthouseId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Lighthouses_TagId",
                table: "Lighthouses");

            migrationBuilder.DropIndex(
                name: "IX_GoogleQueries_LighthouseId",
                table: "GoogleQueries");

            migrationBuilder.DropColumn(
                name: "HasError",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "LatestLog",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "Running",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Lighthouses");

            migrationBuilder.DropColumn(
                name: "LighthouseId",
                table: "GoogleQueries");

            migrationBuilder.RenameColumn(
                name: "LighthouseId",
                table: "Sites",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lighthouses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.CreateTable(
                name: "GoogleQueryLighthouse",
                columns: table => new
                {
                    GoogleQueriesId = table.Column<int>(type: "int", nullable: false),
                    LighthousesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleQueryLighthouse", x => new { x.GoogleQueriesId, x.LighthousesId });
                    table.ForeignKey(
                        name: "FK_GoogleQueryLighthouse_GoogleQueries_GoogleQueriesId",
                        column: x => x.GoogleQueriesId,
                        principalTable: "GoogleQueries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoogleQueryLighthouse_Lighthouses_LighthousesId",
                        column: x => x.LighthousesId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LighthouseSite",
                columns: table => new
                {
                    LighthousesId = table.Column<int>(type: "int", nullable: false),
                    SitesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LighthouseSite", x => new { x.LighthousesId, x.SitesId });
                    table.ForeignKey(
                        name: "FK_LighthouseSite_Lighthouses_LighthousesId",
                        column: x => x.LighthousesId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LighthouseSite_Sites_SitesId",
                        column: x => x.SitesId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoogleQueryLighthouse_LighthousesId",
                table: "GoogleQueryLighthouse",
                column: "LighthousesId");

            migrationBuilder.CreateIndex(
                name: "IX_LighthouseSite_SitesId",
                table: "LighthouseSite",
                column: "SitesId");
        }
    }
}
