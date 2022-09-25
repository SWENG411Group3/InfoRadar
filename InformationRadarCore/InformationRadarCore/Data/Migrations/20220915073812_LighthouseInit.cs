using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class LighthouseInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LighthouseId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lighthouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastVisitorRun = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Frequency = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    MessengerFrequency = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSentMessage = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lighthouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoogleQueries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EngineId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LighthouseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleQueries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoogleQueries_Lighthouses_LighthouseId",
                        column: x => x.LighthouseId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<int>(type: "int", nullable: false),
                    LighthouseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Lighthouses_LighthouseId",
                        column: x => x.LighthouseId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LighthouseId",
                table: "AspNetUsers",
                column: "LighthouseId");

            migrationBuilder.CreateIndex(
                name: "IX_GoogleQueries_LighthouseId",
                table: "GoogleQueries",
                column: "LighthouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Lighthouses_InternalName",
                table: "Lighthouses",
                column: "InternalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LighthouseId",
                table: "Sites",
                column: "LighthouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Lighthouses_LighthouseId",
                table: "AspNetUsers",
                column: "LighthouseId",
                principalTable: "Lighthouses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Lighthouses_LighthouseId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "GoogleQueries");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "Lighthouses");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LighthouseId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LighthouseId",
                table: "AspNetUsers");
        }
    }
}
