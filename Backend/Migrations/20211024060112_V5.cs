using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class V5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Player");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Player",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Game",
                newName: "Name");

            migrationBuilder.InsertData(
                table: "Game",
                columns: new[] { "GameId", "Name" },
                values: new object[] { 1L, "Point game" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Game",
                keyColumn: "GameId",
                keyValue: 1L);

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Player",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Game",
                newName: "Nome");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Player",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
