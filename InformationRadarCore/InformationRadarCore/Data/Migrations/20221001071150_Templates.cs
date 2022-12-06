using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationRadarCore.Data.Migrations
{
    public partial class Templates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseSize",
                table: "Lighthouses");

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LighthouseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateConfigurations_Lighthouses_LighthouseId",
                        column: x => x.LighthouseId,
                        principalTable: "Lighthouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateConfigurations_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateFields_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateLighthouseColumns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateLighthouseColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateLighthouseColumns_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateConfigurations_LighthouseId",
                table: "TemplateConfigurations",
                column: "LighthouseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateConfigurations_TemplateId",
                table: "TemplateConfigurations",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateFields_Name_TemplateId",
                table: "TemplateFields",
                columns: new[] { "Name", "TemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateFields_TemplateId",
                table: "TemplateFields",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateLighthouseColumns_Name_TemplateId",
                table: "TemplateLighthouseColumns",
                columns: new[] { "Name", "TemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateLighthouseColumns_TemplateId",
                table: "TemplateLighthouseColumns",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_InternalName",
                table: "Templates",
                column: "InternalName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateConfigurations");

            migrationBuilder.DropTable(
                name: "TemplateFields");

            migrationBuilder.DropTable(
                name: "TemplateLighthouseColumns");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.AddColumn<int>(
                name: "BaseSize",
                table: "Lighthouses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
