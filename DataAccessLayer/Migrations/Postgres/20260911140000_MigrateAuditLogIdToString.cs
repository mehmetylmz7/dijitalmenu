using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class MigrateAuditLogIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AuditLogs' AND column_name = 'Id' AND data_type = 'integer'
                    ) THEN
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;
                        ALTER TABLE ""AuditLogs"" ALTER COLUMN ""Id"" TYPE character varying(50) USING ""Id""::character varying;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
