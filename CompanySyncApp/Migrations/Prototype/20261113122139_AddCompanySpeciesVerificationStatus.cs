using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanySyncApp.Migrations.Prototype
{
    /// <inheritdoc />
    public partial class AddCompanySpeciesVerificationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                schema: "companysync",
                table: "CompanySpecies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                schema: "companysync",
                table: "CompanySpecies");
        }
    }
}
