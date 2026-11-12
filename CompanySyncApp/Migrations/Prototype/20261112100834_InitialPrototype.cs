using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanySyncApp.Migrations.Prototype
{
    /// <inheritdoc />
    public partial class InitialPrototype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "companysync");

            migrationBuilder.CreateTable(
                name: "AlgaeSpecies",
                schema: "companysync",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesName = table.Column<string>(type: "text", nullable: false),
                    Genus = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    CommonName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgaeSpecies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "companysync",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Headquarters = table.Column<string>(type: "text", nullable: true),
                    ValueChain = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    ProductionProcess = table.Column<string>(type: "text", nullable: true),
                    IndustryApplications = table.Column<string>(type: "text", nullable: true),
                    IndustrySubsectorTags = table.Column<string>(type: "text", nullable: true),
                    Investors = table.Column<string>(type: "text", nullable: true),
                    AssociationFocus = table.Column<string>(type: "text", nullable: true),
                    FoundingYear = table.Column<int>(type: "integer", nullable: true),
                    StartupScaleupStatus = table.Column<string>(type: "text", nullable: true),
                    DisclosedInvestmentsSince2020 = table.Column<decimal>(type: "numeric", nullable: true),
                    DevelopmentStage = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanySpecies",
                schema: "companysync",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeciesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySpecies", x => new { x.CompanyId, x.SpeciesId });
                    table.ForeignKey(
                        name: "FK_CompanySpecies_AlgaeSpecies_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "companysync",
                        principalTable: "AlgaeSpecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanySpecies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "companysync",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanySpecies_SpeciesId",
                schema: "companysync",
                table: "CompanySpecies",
                column: "SpeciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySpecies",
                schema: "companysync");

            migrationBuilder.DropTable(
                name: "AlgaeSpecies",
                schema: "companysync");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "companysync");
        }
    }
}
