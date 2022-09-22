using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class Fix2MRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Lighthouses_LighthouseId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GoogleQueries_Lighthouses_LighthouseId",
                table: "GoogleQueries");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Lighthouses_LighthouseId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_LighthouseId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_GoogleQueries_LighthouseId",
                table: "GoogleQueries");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LighthouseId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LighthouseId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EngineId",
                table: "GoogleQueries");

            migrationBuilder.DropColumn(
                name: "LighthouseId",
                table: "GoogleQueries");

            migrationBuilder.DropColumn(
                name: "LighthouseId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Lighthouses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserLighthouse",
                columns: table => new
                {
                    LighthousesId = table.Column<int>(type: "int", nullable: false),
                    RecipientsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLighthouse", x => new { x.LighthousesId, x.RecipientsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserLighthouse_AspNetUsers_RecipientsId",
                        column: x => x.RecipientsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserLighthouse_Lighthouses_LighthousesId",
                        column: x => x.LighthousesId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_ApplicationUserLighthouse_RecipientsId",
                table: "ApplicationUserLighthouse",
                column: "RecipientsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoogleQueryLighthouse_LighthousesId",
                table: "GoogleQueryLighthouse",
                column: "LighthousesId");

            migrationBuilder.CreateIndex(
                name: "IX_LighthouseSite_SitesId",
                table: "LighthouseSite",
                column: "SitesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserLighthouse");

            migrationBuilder.DropTable(
                name: "GoogleQueryLighthouse");

            migrationBuilder.DropTable(
                name: "LighthouseSite");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Lighthouses");

            migrationBuilder.AddColumn<int>(
                name: "LighthouseId",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineId",
                table: "GoogleQueries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LighthouseId",
                table: "GoogleQueries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LighthouseId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LighthouseId",
                table: "Sites",
                column: "LighthouseId");

            migrationBuilder.CreateIndex(
                name: "IX_GoogleQueries_LighthouseId",
                table: "GoogleQueries",
                column: "LighthouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LighthouseId",
                table: "AspNetUsers",
                column: "LighthouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Lighthouses_LighthouseId",
                table: "AspNetUsers",
                column: "LighthouseId",
                principalTable: "Lighthouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoogleQueries_Lighthouses_LighthouseId",
                table: "GoogleQueries",
                column: "LighthouseId",
                principalTable: "Lighthouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Lighthouses_LighthouseId",
                table: "Sites",
                column: "LighthouseId",
                principalTable: "Lighthouses",
                principalColumn: "Id");
        }
    }
}
