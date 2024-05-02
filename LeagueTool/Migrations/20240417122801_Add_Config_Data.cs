using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueTool.Migrations
{
    /// <inheritdoc />
    public partial class Add_Config_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigName = table.Column<string>(type: "TEXT", nullable: false),
                    ConfigValue = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigDatas_ConfigName",
                table: "ConfigDatas",
                column: "ConfigName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigDatas");
        }
    }
}
