using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp3.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationFilmUtilisateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", nullable: false),
                    Annee = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilmsUtilisateur",
                columns: table => new
                {
                    IdUtilisateur = table.Column<string>(type: "TEXT", nullable: false),
                    IdFilm = table.Column<int>(type: "INTEGER", nullable: false),
                    Vu = table.Column<bool>(type: "INTEGER", nullable: false),
                    Note = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    FilmId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmsUtilisateur", x => new { x.IdUtilisateur, x.IdFilm });
                    table.ForeignKey(
                        name: "FK_FilmsUtilisateur_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmsUtilisateur_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmsUtilisateur_FilmId",
                table: "FilmsUtilisateur",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmsUtilisateur_UserId",
                table: "FilmsUtilisateur",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmsUtilisateur");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "AspNetUsers");
        }
    }
}
