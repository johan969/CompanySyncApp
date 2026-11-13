using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanySyncApp.Migrations.Prototype
{
    /// <inheritdoc />
    public partial class SetCompanySpeciesObservedDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VerificationStatus",
                schema: "companysync",
                table: "CompanySpecies",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            // Befintliga overifierade kopplingar i prototypen kommer från den offentliga CSV-källan.
            migrationBuilder.Sql("""
                UPDATE companysync."CompanySpecies"
                SET "VerificationStatus" = 1
                WHERE "VerificationStatus" = 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VerificationStatus",
                schema: "companysync",
                table: "CompanySpecies",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);
        }
    }
}
