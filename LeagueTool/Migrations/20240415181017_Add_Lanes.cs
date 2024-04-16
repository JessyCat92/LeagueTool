using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueTool.Migrations
{
    /// <inheritdoc />
    public partial class Add_Lanes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lanes",
                table: "ChampionSaves",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lanes",
                table: "ChampionSaves");
        }
    }
}
