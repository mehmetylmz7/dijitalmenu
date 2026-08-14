using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddRestaurantSlugUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM "Restaurants"
                        WHERE "Slug" IS NOT NULL AND "Slug" <> ''
                        GROUP BY "Slug"
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Unique restaurant slug migration cannot continue: duplicate restaurant slugs exist.';
                    END IF;
                END $$;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_Slug",
                table: "Restaurants",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurants_Slug",
                table: "Restaurants");
        }
    }
}
