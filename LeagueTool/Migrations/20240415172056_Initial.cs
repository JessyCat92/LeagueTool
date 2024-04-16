using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueTool.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChampionSaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChampionName = table.Column<string>(type: "TEXT", nullable: false),
                    ChampionKey = table.Column<int>(type: "INTEGER", nullable: false),
                    Played = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tries = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampionSaves", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChampionSaves");
        }
    }
}
