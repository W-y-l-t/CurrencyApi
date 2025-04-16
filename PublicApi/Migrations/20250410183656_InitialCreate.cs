using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fuse8.BackendInternship.PublicApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.CreateTable(
                name: "FavoriteCurrencies",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    BaseCurrency = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteCurrencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteCurrencies_Currency_BaseCurrency",
                schema: "user",
                table: "FavoriteCurrencies",
                columns: new[] { "Currency", "BaseCurrency" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteCurrencies_Name",
                schema: "user",
                table: "FavoriteCurrencies",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteCurrencies",
                schema: "user");
        }
    }
}
