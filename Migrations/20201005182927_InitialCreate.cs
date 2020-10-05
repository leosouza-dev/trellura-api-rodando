using Microsoft.EntityFrameworkCore.Migrations;

namespace Trellura.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cartoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartoes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Cartoes",
                columns: new[] { "Id", "Status", "Titulo" },
                values: new object[] { 1, 1, "Tarefa 1" });

            migrationBuilder.InsertData(
                table: "Cartoes",
                columns: new[] { "Id", "Status", "Titulo" },
                values: new object[] { 2, 1, "Segunda tarefa" });

            migrationBuilder.InsertData(
                table: "Cartoes",
                columns: new[] { "Id", "Status", "Titulo" },
                values: new object[] { 3, 1, "Tarefa 3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cartoes");
        }
    }
}
