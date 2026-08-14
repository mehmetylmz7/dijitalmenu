using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantSlugUniqueIndexSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1 FROM [Restaurants]
                    WHERE [Slug] IS NOT NULL AND [Slug] <> ''
                    GROUP BY [Slug]
                    HAVING COUNT(*) > 1
                )
                    THROW 51010, 'Unique restaurant slug migration cannot continue: duplicate restaurant slugs exist.', 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_Slug",
                table: "Restaurants",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");
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
