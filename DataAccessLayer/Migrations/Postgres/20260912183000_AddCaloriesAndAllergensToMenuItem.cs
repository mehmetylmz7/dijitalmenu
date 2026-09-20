using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddCaloriesAndAllergensToMenuItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""MenuItems"" ADD COLUMN IF NOT EXISTS ""Calories"" integer;");
            migrationBuilder.Sql(@"ALTER TABLE ""MenuItems"" ADD COLUMN IF NOT EXISTS ""Allergens"" character varying(500);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allergens",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "MenuItems");
        }
    }
}
