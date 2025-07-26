using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookBooks.Migrations
{
    /// <inheritdoc />
    public partial class RenomearColunasLivro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Livro",
                table: "Livros",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "Ano",
                table: "Livros",
                newName: "AnoPublicacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Livros",
                newName: "Livro");

            migrationBuilder.RenameColumn(
                name: "AnoPublicacao",
                table: "Livros",
                newName: "Ano");
        }
    }
}
