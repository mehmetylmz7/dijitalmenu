using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class EnsureEntityColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Restaurants"" ADD COLUMN IF NOT EXISTS ""ImportantNotice"" character varying(1000);");
            migrationBuilder.Sql(@"ALTER TABLE ""Restaurants"" ADD COLUMN IF NOT EXISTS ""WorkingHours"" character varying(200);");
            migrationBuilder.Sql(@"ALTER TABLE ""Restaurants"" ADD COLUMN IF NOT EXISTS ""InstagramUrl"" character varying(2048);");
            migrationBuilder.Sql(@"ALTER TABLE ""Themes"" ADD COLUMN IF NOT EXISTS ""IsActive"" boolean NOT NULL DEFAULT true;");
            migrationBuilder.Sql(@"ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""DisplayOrder"" integer NOT NULL DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE ""MenuItems"" ADD COLUMN IF NOT EXISTS ""DisplayOrder"" integer NOT NULL DEFAULT 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
