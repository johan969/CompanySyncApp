using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Arb_Int.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessRelationshipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificationSchemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StandardOwner = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StandardVersion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PublicUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationSchemes", x => x.Id);
                    table.CheckConstraint("CK_CertificationSchemes_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConstituentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstituentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactFunctionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactFunctionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Continents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContinentCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Id_Old = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: true),
                    DecimalDigits = table.Column<int>(type: "integer", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: true),
                    LatestExchangeRateChangeId = table.Column<long>(type: "bigint", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataMigration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedWithSuccess = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMigration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsLotSpecificByDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CanExpire = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false, defaultValue: "Organisation"),
                    Kind = table.Column<string>(type: "text", nullable: false, defaultValue: "Generic"),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BaseUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncotermRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncotermRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchingRuleSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LatestVersionNumber = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingRuleSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsRawMaterial = table.Column<bool>(type: "boolean", nullable: false),
                    ParentMaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                    table.CheckConstraint("CK_MaterialTypes_Parent_NotSelf", "\"ParentMaterialTypeId\" IS NULL OR \"ParentMaterialTypeId\" <> \"Id\"");
                    table.ForeignKey(
                        name: "FK_MaterialTypes_MaterialTypes_ParentMaterialTypeId",
                        column: x => x.ParentMaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    ReferenceKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Description = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Use = table.Column<string>(type: "text", nullable: false),
                    Parameters = table.Column<string>(type: "text", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedReason = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentMilestoneTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentMilestoneTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Taxon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Rank = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    PathKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ParentTaxonId = table.Column<Guid>(type: "uuid", nullable: true),
                    AcceptedTaxonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxon", x => x.Id);
                    table.CheckConstraint("CK_Taxon_AcceptedTaxonId_NotSelf", "\"AcceptedTaxonId\" IS NULL OR \"AcceptedTaxonId\" <> \"Id\"");
                    table.CheckConstraint("CK_Taxon_ParentTaxonId_NotSelf", "\"ParentTaxonId\" IS NULL OR \"ParentTaxonId\" <> \"Id\"");
                    table.ForeignKey(
                        name: "FK_Taxon_Taxon_AcceptedTaxonId",
                        column: x => x.AcceptedTaxonId,
                        principalTable: "Taxon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Taxon_Taxon_ParentTaxonId",
                        column: x => x.ParentTaxonId,
                        principalTable: "Taxon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeExecutionCheckpointTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefaultCompletionMode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DefaultIsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultSortOrder = table.Column<int>(type: "integer", nullable: false),
                    DefaultAutomationHandlerKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeExecutionCheckpointTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasure",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Dimension = table.Column<string>(type: "text", nullable: true, defaultValue: "Mass"),
                    FactorToBaseUnit = table.Column<decimal>(type: "numeric", nullable: false),
                    IsBaseUnit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    IsPasswordChangeRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsInactivated = table.Column<bool>(type: "boolean", nullable: false),
                    InactivatedTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InactivatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InactivatedByOnBehalfOfUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Waters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    FeatureType = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    ParentWaterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waters_Waters_ParentWaterId",
                        column: x => x.ParentWaterId,
                        principalTable: "Waters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkerHeartbeats",
                columns: table => new
                {
                    InstanceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StartedUtc = table.Column<DateTimeOffset>(type: "timestamp(0) with time zone", precision: 0, nullable: false),
                    LastSeenUtc = table.Column<DateTimeOffset>(type: "timestamp(0) with time zone", precision: 0, nullable: false),
                    Node = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerHeartbeats", x => x.InstanceId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationshipStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessRelationshipTypeId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipStages", x => x.Id);
                    table.UniqueConstraint("AK_BusinessRelationshipStages_BusinessRelationshipTypeId_Id", x => new { x.BusinessRelationshipTypeId, x.Id });
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipStages_BusinessRelationshipTypes_Busine~",
                        column: x => x.BusinessRelationshipTypeId,
                        principalTable: "BusinessRelationshipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CharacteristicTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characteristics_CharacteristicTypes_CharacteristicTypeId",
                        column: x => x.CharacteristicTypeId,
                        principalTable: "CharacteristicTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Constituents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ConstituentTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constituents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Constituents_ConstituentTypes_ConstituentTypeId",
                        column: x => x.ConstituentTypeId,
                        principalTable: "ConstituentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsEUCountry = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DefaultTimeZone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Alpha2Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Alpha3Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ISO3166Numeric = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    DefaultVAT = table.Column<double>(type: "double precision", precision: 5, scale: 2, nullable: true),
                    CurrencyId = table.Column<int>(type: "integer", nullable: true),
                    ContinentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Continents_ContinentId",
                        column: x => x.ContinentId,
                        principalTable: "Continents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Countries_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRateChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    RateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric(19,8)", precision: 19, scale: 8, nullable: false),
                    PreviousExchangeRate = table.Column<decimal>(type: "numeric(19,8)", precision: 19, scale: 8, nullable: true),
                    RetrievedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRateChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRateChanges_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplateVariant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Culture = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SubjectTemplate = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    BodyHtmlTemplate = table.Column<string>(type: "character varying(65536)", maxLength: 65536, nullable: false),
                    BodyTextTemplate = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailTemplateVariant_EmailTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "EmailTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingRuleSetVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingRuleSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    ConfigurationJson = table.Column<string>(type: "jsonb", nullable: false),
                    ConfigurationHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EffectiveFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EffectiveToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublishedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PublishedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingRuleSetVersions", x => x.Id);
                    table.CheckConstraint("CK_MatchingRuleSetVersions_EffectiveWindow", "\"EffectiveToUtc\" IS NULL\nOR \"EffectiveFromUtc\" IS NULL\nOR \"EffectiveToUtc\" >= \"EffectiveFromUtc\"");
                    table.CheckConstraint("CK_MatchingRuleSetVersions_VersionNumber", "\"VersionNumber\" > 0");
                    table.ForeignKey(
                        name: "FK_MatchingRuleSetVersions_MatchingRuleSets_MatchingRuleSetId",
                        column: x => x.MatchingRuleSetId,
                        principalTable: "MatchingRuleSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartyExternalReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceRecordKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    FirstSeenUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSeenUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyExternalReferences", x => x.Id);
                    table.CheckConstraint("CK_PartyExternalReferences_SeenDates", "\"LastSeenUtc\" >= \"FirstSeenUtc\"");
                    table.ForeignKey(
                        name: "FK_PartyExternalReferences_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GivenName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FamilyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PreferredName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Honorific = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PreferredLanguageCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Parties_Id",
                        column: x => x.Id,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTransformations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProcessTypeId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTransformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialTransformations_ProcessTypes_ProcessTypeId",
                        column: x => x.ProcessTypeId,
                        principalTable: "ProcessTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BiologicalSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    RecordKind = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false, defaultValue: "Unknown"),
                    DeclaredRank = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    TaxonId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentBiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegulatoryRisk = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    RegulatoryRiskReason = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    TypicalHabitat = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    DistributionSummary = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiologicalSources", x => x.Id);
                    table.CheckConstraint("CK_BiologicalSources_Parent_NotSelf", "\"ParentBiologicalSourceId\" IS NULL OR \"ParentBiologicalSourceId\" <> \"Id\"");
                    table.ForeignKey(
                        name: "FK_BiologicalSources_BiologicalSources_ParentBiologicalSourceId",
                        column: x => x.ParentBiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BiologicalSources_Taxon_TaxonId",
                        column: x => x.TaxonId,
                        principalTable: "Taxon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ReasonRevoked = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CareOf = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PostOfficeBox = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLine2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLine3 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Locality = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdministrativeArea = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.CheckConstraint("CK_Addresses_HasAddressData", "\"PostOfficeBox\" IS NOT NULL\nOR \"AddressLine1\" IS NOT NULL\nOR \"AddressLine2\" IS NOT NULL\nOR \"AddressLine3\" IS NOT NULL\nOR \"PostalCode\" IS NOT NULL\nOR \"Locality\" IS NOT NULL\nOR \"AdministrativeArea\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CertificationSchemeJurisdictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificationSchemeId = table.Column<int>(type: "integer", nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationSchemeJurisdictions", x => x.Id);
                    table.UniqueConstraint("AK_CertificationSchemeJurisdictions_Id_CertificationSchemeId", x => new { x.Id, x.CertificationSchemeId });
                    table.ForeignKey(
                        name: "FK_CertificationSchemeJurisdictions_CertificationSchemes_Certi~",
                        column: x => x.CertificationSchemeId,
                        principalTable: "CertificationSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificationSchemeJurisdictions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CountryWater",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    WaterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryWater", x => new { x.CountryId, x.WaterId });
                    table.ForeignKey(
                        name: "FK_CountryWater_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryWater_Waters_WaterId",
                        column: x => x.WaterId,
                        principalTable: "Waters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeographicLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Scope = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "CountryOrTerritory"),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    WaterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Subregion = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    LocationKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeographicLocations", x => x.Id);
                    table.CheckConstraint("CK_GeographicLocations_ScopeCountry", "(\n    \"Scope\" = 'Global'\n    AND \"CountryId\" IS NULL\n)\nOR\n(\n    \"Scope\" = 'CountryOrTerritory'\n    AND \"CountryId\" IS NOT NULL\n)");
                    table.ForeignKey(
                        name: "FK_GeographicLocations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeographicLocations_Waters_WaterId",
                        column: x => x.WaterId,
                        principalTable: "Waters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NormalizedLegalName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TradingName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OrganizationKind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Unknown"),
                    IsInternal = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    RegistrationCountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentOrganizationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.CheckConstraint("CK_Organizations_Parent_NotSelf", "\"ParentOrganizationId\" IS NULL OR \"ParentOrganizationId\" <> \"Id\"");
                    table.ForeignKey(
                        name: "FK_Organizations_Countries_RegistrationCountryId",
                        column: x => x.RegistrationCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organizations_Organizations_ParentOrganizationId",
                        column: x => x.ParentOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organizations_Parties_Id",
                        column: x => x.Id,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderEmailTemplateMapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    ProviderTemplateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderEmailTemplateMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderEmailTemplateMapping_EmailTemplateVariant_TemplateV~",
                        column: x => x.TemplateVariantId,
                        principalTable: "EmailTemplateVariant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialCharacteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialCharacteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialCharacteristics_Characteristics_CharacteristicId",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialCharacteristics_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialConstituents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConstituentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceLevel = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, defaultValue: "None"),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialConstituents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialConstituents_Constituents_ConstituentId",
                        column: x => x.ConstituentId,
                        principalTable: "Constituents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialConstituents_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReferences_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialReferences_ReferenceEntries_ReferenceEntryId",
                        column: x => x.ReferenceEntryId,
                        principalTable: "ReferenceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedSku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Draft"),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesUnitQuantity = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 1m),
                    SalesUnitOfMeasureId = table.Column<int>(type: "integer", nullable: false),
                    NetContentQuantity = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    NetContentUnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    AvailableFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DiscontinuedDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.CheckConstraint("CK_Products_DiscontinuedDateUtc", "\"DiscontinuedDateUtc\" IS NULL\nOR \"AvailableFromUtc\" IS NULL\nOR \"DiscontinuedDateUtc\" >= \"AvailableFromUtc\"");
                    table.CheckConstraint("CK_Products_NetContent_Complete", "(\n    \"NetContentQuantity\" IS NULL\n    AND \"NetContentUnitOfMeasureId\" IS NULL\n)\nOR\n(\n    \"NetContentQuantity\" IS NOT NULL\n    AND \"NetContentQuantity\" > 0\n    AND \"NetContentUnitOfMeasureId\" IS NOT NULL\n)");
                    table.CheckConstraint("CK_Products_SalesUnitQuantity_Positive", "\"SalesUnitQuantity\" > 0");
                    table.ForeignKey(
                        name: "FK_Products_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_UnitsOfMeasure_NetContentUnitOfMeasureId",
                        column: x => x.NetContentUnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_UnitsOfMeasure_SalesUnitOfMeasureId",
                        column: x => x.SalesUnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTransformationInputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialTransformationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Quantity = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    Percentage = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTransformationInputs", x => x.Id);
                    table.CheckConstraint("CK_MaterialTransformationInputs_Percentage", "\"Percentage\" IS NULL OR (\"Percentage\" >= 0 AND \"Percentage\" <= 100)");
                    table.CheckConstraint("CK_MaterialTransformationInputs_Quantity", "\"Quantity\" IS NULL OR \"Quantity\" >= 0");
                    table.ForeignKey(
                        name: "FK_MaterialTransformationInputs_MaterialTransformations_Materi~",
                        column: x => x.MaterialTransformationId,
                        principalTable: "MaterialTransformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialTransformationInputs_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialTransformationInputs_UnitsOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTransformationOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialTransformationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Quantity = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    YieldPercent = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTransformationOutputs", x => x.Id);
                    table.CheckConstraint("CK_MaterialTransformationOutputs_Quantity", "\"Quantity\" IS NULL OR \"Quantity\" >= 0");
                    table.CheckConstraint("CK_MaterialTransformationOutputs_Yield", "\"YieldPercent\" IS NULL OR (\"YieldPercent\" >= 0 AND \"YieldPercent\" <= 100)");
                    table.ForeignKey(
                        name: "FK_MaterialTransformationOutputs_MaterialTransformations_Mater~",
                        column: x => x.MaterialTransformationId,
                        principalTable: "MaterialTransformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialTransformationOutputs_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialTransformationOutputs_UnitsOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BiologicalSourceCharacteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiologicalSourceCharacteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceCharacteristics_BiologicalSources_Biologica~",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceCharacteristics_Characteristics_Characteris~",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BiologicalSourceConstituents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConstituentId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceLevel = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, defaultValue: "None"),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiologicalSourceConstituents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceConstituents_BiologicalSources_BiologicalSo~",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceConstituents_Constituents_ConstituentId",
                        column: x => x.ConstituentId,
                        principalTable: "Constituents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BiologicalSourceNames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    NameType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, defaultValue: "und"),
                    IsPreferred = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiologicalSourceNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceNames_BiologicalSources_BiologicalSourceId",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BiologicalSourceReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiologicalSourceReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceReferences_BiologicalSources_BiologicalSour~",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceReferences_ReferenceEntries_ReferenceEntryId",
                        column: x => x.ReferenceEntryId,
                        principalTable: "ReferenceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialBiologicalSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    ProportionPercent = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialBiologicalSources", x => x.Id);
                    table.CheckConstraint("CK_MaterialBiologicalSources_Proportion", "\"ProportionPercent\" IS NULL OR (\"ProportionPercent\" >= 0 AND \"ProportionPercent\" <= 100)");
                    table.ForeignKey(
                        name: "FK_MaterialBiologicalSources_BiologicalSources_BiologicalSourc~",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialBiologicalSources_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsageType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attention = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyAddresses", x => x.Id);
                    table.CheckConstraint("CK_PartyAddresses_ValidDates", "\"ValidToUtc\" IS NULL OR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_PartyAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartyAddresses_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BiologicalSourceRegions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeographicLocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiologicalSourceRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceRegions_BiologicalSources_BiologicalSourceId",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiologicalSourceRegions_GeographicLocations_GeographicLocat~",
                        column: x => x.GeographicLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialOrigins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeographicLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialOrigins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialOrigins_GeographicLocations_GeographicLocationId",
                        column: x => x.GeographicLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialOrigins_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwningOrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CounterpartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipTypeId = table.Column<int>(type: "integer", nullable: false),
                    BusinessRelationshipStageId = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, defaultValue: "Normal"),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    LeadSource = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FirstContactedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastContactedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    NextActionUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClosedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationships", x => x.Id);
                    table.CheckConstraint("CK_BusinessRelationships_ClosedDate", "\"ClosedUtc\" IS NULL OR \"ClosedUtc\" >= \"FirstObservedUtc\"");
                    table.CheckConstraint("CK_BusinessRelationships_ContactDates", "\"LastContactedUtc\" IS NULL\nOR \"FirstContactedUtc\" IS NULL\nOR \"LastContactedUtc\" >= \"FirstContactedUtc\"");
                    table.CheckConstraint("CK_BusinessRelationships_Score", "\"Score\" IS NULL OR (\"Score\" >= 0 AND \"Score\" <= 100)");
                    table.ForeignKey(
                        name: "FK_BusinessRelationships_BusinessRelationshipStages_BusinessRe~",
                        columns: x => new { x.BusinessRelationshipTypeId, x.BusinessRelationshipStageId },
                        principalTable: "BusinessRelationshipStages",
                        principalColumns: new[] { "BusinessRelationshipTypeId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelationships_BusinessRelationshipTypes_BusinessRel~",
                        column: x => x.BusinessRelationshipTypeId,
                        principalTable: "BusinessRelationshipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelationships_Organizations_OwningOrganizationId",
                        column: x => x.OwningOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelationships_Parties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationIdentifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Scheme = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NormalizedValue = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Issuer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssuingCountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IdentifierKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationIdentifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationIdentifiers_Countries_IssuingCountryId",
                        column: x => x.IssuingCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationIdentifiers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPeople",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationshipType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    JobTitle = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Department = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPrimaryContact = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPeople", x => x.Id);
                    table.UniqueConstraint("AK_OrganizationPeople_Id_PersonId", x => new { x.Id, x.PersonId });
                    table.CheckConstraint("CK_OrganizationPeople_ValidDates", "\"ValidToUtc\" IS NULL OR \"ValidFromUtc\" IS NULL OR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_OrganizationPeople_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPeople_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductCharacteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCharacteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCharacteristics_Characteristics_CharacteristicId",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCharacteristics_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductIdentifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Scheme = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Issuer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IdentifierKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIdentifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductIdentifiers_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReferences_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReferences_ReferenceEntries_ReferenceEntryId",
                        column: x => x.ReferenceEntryId,
                        principalTable: "ReferenceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationshipBiologicalSourceClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeclaredName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NormalizedDeclaredName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelationshipKind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    MappingStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unmapped"),
                    MappingConfidence = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    VerificationStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Unverified"),
                    FirstObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastVerifiedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipBiologicalSourceClaims", x => x.Id);
                    table.UniqueConstraint("AK_BusinessRelationshipBiologicalSourceClaims_Id_BusinessRelat~", x => new { x.Id, x.BusinessRelationshipId });
                    table.CheckConstraint("CK_BusinessRelationshipSourceClaims_Confidence", "\"MappingConfidence\" IS NULL\nOR\n(\n    \"MappingConfidence\" >= 0\n    AND \"MappingConfidence\" <= 1\n)");
                    table.CheckConstraint("CK_BusinessRelationshipSourceClaims_ConfirmedMapping", "\"MappingStatus\" <> 'Confirmed'\nOR \"BiologicalSourceId\" IS NOT NULL");
                    table.CheckConstraint("CK_BusinessRelationshipSourceClaims_ObservedDates", "\"LastObservedUtc\" >= \"FirstObservedUtc\"");
                    table.CheckConstraint("CK_BusinessRelationshipSourceClaims_VerifiedDate", "\"LastVerifiedUtc\" IS NULL\nOR \"LastVerifiedUtc\" >= \"FirstObservedUtc\"");
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipBiologicalSourceClaims_BiologicalSource~",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipBiologicalSourceClaims_BusinessRelation~",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationshipStageHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStageId = table.Column<int>(type: "integer", nullable: true),
                    ToStageId = table.Column<int>(type: "integer", nullable: false),
                    ChangedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipStageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipStageHistory_BusinessRelationshipStages~",
                        column: x => x.FromStageId,
                        principalTable: "BusinessRelationshipStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipStageHistory_BusinessRelationshipStage~1",
                        column: x => x.ToStageId,
                        principalTable: "BusinessRelationshipStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipStageHistory_BusinessRelationships_Busi~",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerDemands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    KnowledgeLevel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastConfirmedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerDemands", x => x.Id);
                    table.UniqueConstraint("AK_BuyerDemands_Id_BusinessRelationshipId", x => new { x.Id, x.BusinessRelationshipId });
                    table.CheckConstraint("CK_BuyerDemands_Confidence", "\"Confidence\" IS NULL\nOR (\"Confidence\" >= 0 AND \"Confidence\" <= 1)");
                    table.CheckConstraint("CK_BuyerDemands_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_BuyerDemands_BusinessRelationships_BusinessRelationshipId",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CrmActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegardingPartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Planned"),
                    Direction = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, defaultValue: "None"),
                    Subject = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: false),
                    OccurredUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DueUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalRecordId = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmActivities", x => x.Id);
                    table.CheckConstraint("CK_CrmActivities_Completed", "\"Status\" <> 'Completed'\nOR \"CompletedUtc\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_CrmActivities_BusinessRelationships_BusinessRelationshipId",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrmActivities_Parties_RegardingPartyId",
                        column: x => x.RegardingPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisclosurePackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AudienceBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    Direction = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SourceEntityType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SourceEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    LatestRevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisclosurePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisclosurePackages_BusinessRelationships_AudienceBusinessRe~",
                        column: x => x.AudienceBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Mode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsConsolidated = table.Column<bool>(type: "boolean", nullable: false),
                    FreightForwarderBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlannedPickupAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualPickupAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PlannedDeliveryAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualDeliveryAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MasterTrackingNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Addresses_DestinationAddressId",
                        column: x => x.DestinationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Addresses_OriginAddressId",
                        column: x => x.OriginAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_BusinessRelationships_FreightForwarderBusinessRel~",
                        column: x => x.FreightForwarderBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_GeographicLocations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_GeographicLocations_OriginLocationId",
                        column: x => x.OriginLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierMaterialOfferings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    OfferingKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    SupplierSku = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NormalizedSupplierSku = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DeclaredFormat = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    OfferedMaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    MappedMaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    MappingStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unmapped"),
                    MappingConfidence = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    MappingNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Observed"),
                    FirstObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastVerifiedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierMaterialOfferings", x => x.Id);
                    table.UniqueConstraint("AK_SupplierMaterialOfferings_Id_BusinessRelationshipId", x => new { x.Id, x.BusinessRelationshipId });
                    table.CheckConstraint("CK_SupplierMaterialOfferings_ConfirmedMaterial", "\"MappingStatus\" <> 'Confirmed'\nOR \"MappedMaterialId\" IS NOT NULL");
                    table.CheckConstraint("CK_SupplierMaterialOfferings_MappingConfidence", "\"MappingConfidence\" IS NULL\nOR\n(\n    \"MappingConfidence\" >= 0\n    AND \"MappingConfidence\" <= 1\n)");
                    table.CheckConstraint("CK_SupplierMaterialOfferings_ObservedDates", "\"LastObservedUtc\" >= \"FirstObservedUtc\"");
                    table.CheckConstraint("CK_SupplierMaterialOfferings_VerifiedDate", "\"LastVerifiedUtc\" IS NULL\nOR \"LastVerifiedUtc\" >= \"FirstObservedUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferings_BusinessRelationships_BusinessRel~",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferings_MaterialTypes_OfferedMaterialType~",
                        column: x => x.OfferedMaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferings_Materials_MappedMaterialId",
                        column: x => x.MappedMaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProcessCapabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    CapabilityKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeclaredProcessName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NormalizedDeclaredProcessName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProcessTypeId = table.Column<int>(type: "integer", nullable: true),
                    InputMaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    OutputMaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    MappingStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unmapped"),
                    MappingConfidence = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Observed"),
                    VerificationStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Unverified"),
                    FirstObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastVerifiedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProcessCapabilities", x => x.Id);
                    table.UniqueConstraint("AK_SupplierProcessCapabilities_Id_BusinessRelationshipId", x => new { x.Id, x.BusinessRelationshipId });
                    table.CheckConstraint("CK_SupplierProcessCapabilities_Confidence", "\"MappingConfidence\" IS NULL\nOR\n(\n    \"MappingConfidence\" >= 0\n    AND \"MappingConfidence\" <= 1\n)");
                    table.CheckConstraint("CK_SupplierProcessCapabilities_ConfirmedMapping", "\"MappingStatus\" <> 'Confirmed'\nOR \"ProcessTypeId\" IS NOT NULL");
                    table.CheckConstraint("CK_SupplierProcessCapabilities_ObservedDates", "\"LastObservedUtc\" >= \"FirstObservedUtc\"");
                    table.CheckConstraint("CK_SupplierProcessCapabilities_VerifiedDate", "\"LastVerifiedUtc\" IS NULL\nOR \"LastVerifiedUtc\" >= \"FirstObservedUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilities_BusinessRelationships_BusinessR~",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilities_MaterialTypes_InputMaterialType~",
                        column: x => x.InputMaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilities_MaterialTypes_OutputMaterialTyp~",
                        column: x => x.OutputMaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilities_ProcessTypes_ProcessTypeId",
                        column: x => x.ProcessTypeId,
                        principalTable: "ProcessTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationshipContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipContacts_BusinessRelationships_Business~",
                        column: x => x.BusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipContacts_OrganizationPeople_Organizatio~",
                        column: x => x.OrganizationPersonId,
                        principalTable: "OrganizationPeople",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPersonFunctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactFunctionTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPersonFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPersonFunctions_ContactFunctionTypes_ContactFun~",
                        column: x => x.ContactFunctionTypeId,
                        principalTable: "ContactFunctionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPersonFunctions_OrganizationPeople_Organization~",
                        column: x => x.OrganizationPersonId,
                        principalTable: "OrganizationPeople",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyContactPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationPersonId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    NormalizedValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Extension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PermissionStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyContactPoints", x => x.Id);
                    table.CheckConstraint("CK_PartyContactPoints_ValidDates", "\"ValidToUtc\" IS NULL OR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_PartyContactPoints_OrganizationPeople_OrganizationPersonId_~",
                        columns: x => new { x.OrganizationPersonId, x.PartyId },
                        principalTable: "OrganizationPeople",
                        principalColumns: new[] { "Id", "PersonId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartyContactPoints_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationshipBiologicalSourceClaimEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipBiologicalSourceClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    SourceSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceRecordKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipBiologicalSourceClaimEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipBiologicalSourceClaimEvidence_BusinessR~",
                        column: x => x.BusinessRelationshipBiologicalSourceClaimId,
                        principalTable: "BusinessRelationshipBiologicalSourceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRelationshipBiologicalSourceClaimRegions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipBiologicalSourceClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeographicLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRelationshipBiologicalSourceClaimRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipBiologicalSourceClaimRegions_BusinessRe~",
                        column: x => x.BusinessRelationshipBiologicalSourceClaimId,
                        principalTable: "BusinessRelationshipBiologicalSourceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessRelationshipBiologicalSourceClaimRegions_Geographic~",
                        column: x => x.GeographicLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerDemandScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerDemandId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiologicalSourceClaimId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    IntendedUseCharacteristicId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeclaredFormat = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IntendedUseText = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    TypicalQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    TypicalQuantityUnitId = table.Column<int>(type: "integer", nullable: true),
                    RecurrenceDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerDemandScopes", x => x.Id);
                    table.CheckConstraint("CK_BuyerDemandScopes_Quantity", "(\"TypicalQuantity\" IS NULL AND \"TypicalQuantityUnitId\" IS NULL)\nOR (\"TypicalQuantity\" > 0 AND \"TypicalQuantityUnitId\" IS NOT NULL)");
                    table.CheckConstraint("CK_BuyerDemandScopes_Target", "num_nonnulls(\"BiologicalSourceClaimId\", \"MaterialId\", \"MaterialTypeId\", \"DeclaredFormat\",\n    \"IntendedUseCharacteristicId\", \"IntendedUseText\") >= 1");
                    table.ForeignKey(
                        name: "FK_BuyerDemandScopes_BusinessRelationshipBiologicalSourceClaim~",
                        columns: x => new { x.BiologicalSourceClaimId, x.BusinessRelationshipId },
                        principalTable: "BusinessRelationshipBiologicalSourceClaims",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerDemandScopes_BuyerDemands_BuyerDemandId_BusinessRelati~",
                        columns: x => new { x.BuyerDemandId, x.BusinessRelationshipId },
                        principalTable: "BuyerDemands",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerDemandScopes_Characteristics_IntendedUseCharacteristic~",
                        column: x => x.IntendedUseCharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerDemandScopes_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerDemandScopes_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerDemandScopes_UnitsOfMeasure_TypicalQuantityUnitId",
                        column: x => x.TypicalQuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerDemandId = table.Column<Guid>(type: "uuid", nullable: true),
                    RfqNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Priority = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Visibility = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BuyerReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LatestRevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    OpenedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqs", x => x.Id);
                    table.UniqueConstraint("AK_BuyerRfqs_Id_BuyerBusinessRelationshipId", x => new { x.Id, x.BuyerBusinessRelationshipId });
                    table.ForeignKey(
                        name: "FK_BuyerRfqs_BusinessRelationships_BuyerBusinessRelationshipId",
                        column: x => x.BuyerBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqs_BuyerDemands_BuyerDemandId_BuyerBusinessRelations~",
                        columns: x => new { x.BuyerDemandId, x.BuyerBusinessRelationshipId },
                        principalTable: "BuyerDemands",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerDemandEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerDemandId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceKind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Position = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReferenceEntryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CrmActivityId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceUri = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SourceExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Excerpt = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    Confidence = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    ObservedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EvidenceHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerDemandEvidence", x => x.Id);
                    table.UniqueConstraint("AK_BuyerDemandEvidence_Id_BuyerDemandId", x => new { x.Id, x.BuyerDemandId });
                    table.CheckConstraint("CK_BuyerDemandEvidence_Confidence", "\"Confidence\" IS NULL\nOR (\"Confidence\" >= 0 AND \"Confidence\" <= 1)");
                    table.ForeignKey(
                        name: "FK_BuyerDemandEvidence_BuyerDemands_BuyerDemandId",
                        column: x => x.BuyerDemandId,
                        principalTable: "BuyerDemands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerDemandEvidence_CrmActivities_CrmActivityId",
                        column: x => x.CrmActivityId,
                        principalTable: "CrmActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerDemandEvidence_ReferenceEntries_ReferenceEntryId",
                        column: x => x.ReferenceEntryId,
                        principalTable: "ReferenceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CrmActivityParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CrmActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmActivityParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmActivityParticipants_CrmActivities_CrmActivityId",
                        column: x => x.CrmActivityId,
                        principalTable: "CrmActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrmActivityParticipants_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisclosurePackageRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisclosurePackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ModeratedSummary = table.Column<string>(type: "character varying(16000)", maxLength: 16000, nullable: true),
                    SnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ApprovedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SentAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisclosurePackageRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisclosurePackageRevisions_DisclosurePackages_DisclosurePac~",
                        column: x => x.DisclosurePackageId,
                        principalTable: "DisclosurePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentLegs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Mode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CarrierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    FromLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    FromAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlannedDepartureAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualDepartureAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PlannedArrivalAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualArrivalAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TrackingNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BookingReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentLegs", x => x.Id);
                    table.UniqueConstraint("AK_ShipmentLegs_Id_ShipmentId", x => new { x.Id, x.ShipmentId });
                    table.ForeignKey(
                        name: "FK_ShipmentLegs_Addresses_FromAddressId",
                        column: x => x.FromAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLegs_Addresses_ToAddressId",
                        column: x => x.ToAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLegs_BusinessRelationships_CarrierBusinessRelations~",
                        column: x => x.CarrierBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLegs_GeographicLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLegs_GeographicLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLegs_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    LotNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SupplierLotNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    HarvestedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ProducedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BestBeforeUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CurrentLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarehouseReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLots", x => x.Id);
                    table.UniqueConstraint("AK_InventoryLots_Id_SupplierBusinessRelationshipId", x => new { x.Id, x.SupplierBusinessRelationshipId });
                    table.CheckConstraint("CK_InventoryLots_Quantities", "\"TotalQuantity\" >= 0\nAND \"AvailableQuantity\" >= 0\nAND \"ReservedQuantity\" >= 0\nAND \"AvailableQuantity\" + \"ReservedQuantity\" <= \"TotalQuantity\"");
                    table.ForeignKey(
                        name: "FK_InventoryLots_BusinessRelationships_SupplierBusinessRelatio~",
                        column: x => x.SupplierBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLots_GeographicLocations_CurrentLocationId",
                        column: x => x.CurrentLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLots_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLots_SupplierMaterialOfferings_SupplierMaterialOff~",
                        columns: x => new { x.SupplierMaterialOfferingId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierMaterialOfferings",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLots_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierMaterialOfferingBiologicalSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipBiologicalSourceClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    ProportionPercent = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierMaterialOfferingBiologicalSources", x => x.Id);
                    table.CheckConstraint("CK_SupplierOfferingSources_Proportion", "\"ProportionPercent\" IS NULL\nOR\n(\n    \"ProportionPercent\" >= 0\n    AND \"ProportionPercent\" <= 100\n)");
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingBiologicalSources_BusinessRelations~",
                        columns: x => new { x.BusinessRelationshipBiologicalSourceClaimId, x.BusinessRelationshipId },
                        principalTable: "BusinessRelationshipBiologicalSourceClaims",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingBiologicalSources_SupplierMaterialO~",
                        columns: x => new { x.SupplierMaterialOfferingId, x.BusinessRelationshipId },
                        principalTable: "SupplierMaterialOfferings",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierMaterialOfferingCharacteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierMaterialOfferingCharacteristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingCharacteristics_Characteristics_Cha~",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingCharacteristics_SupplierMaterialOff~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierMaterialOfferingEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    SourceSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceRecordKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    FirstObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierMaterialOfferingEvidence", x => x.Id);
                    table.CheckConstraint("CK_SupplierOfferingEvidence_ObservedDates", "\"LastObservedUtc\" >= \"FirstObservedUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingEvidence_SupplierMaterialOfferings_~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierMaterialOfferingOrigins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeographicLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierMaterialOfferingOrigins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingOrigins_GeographicLocations_Geograp~",
                        column: x => x.GeographicLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierMaterialOfferingOrigins_SupplierMaterialOfferings_S~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProcessCapabilityBiologicalSourceScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessRelationshipBiologicalSourceClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProcessCapabilityBiologicalSourceScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilityBiologicalSourceScopes_BusinessRel~",
                        columns: x => new { x.BusinessRelationshipBiologicalSourceClaimId, x.BusinessRelationshipId },
                        principalTable: "BusinessRelationshipBiologicalSourceClaims",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilityBiologicalSourceScopes_SupplierPro~",
                        columns: x => new { x.SupplierProcessCapabilityId, x.BusinessRelationshipId },
                        principalTable: "SupplierProcessCapabilities",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProcessCapabilityEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvidenceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Unknown"),
                    SourceSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceRecordKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    FirstObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastObservedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProcessCapabilityEvidence", x => x.Id);
                    table.CheckConstraint("CK_SupplierProcessCapabilityEvidence_ObservedDates", "\"LastObservedUtc\" >= \"FirstObservedUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierProcessCapabilityEvidence_SupplierProcessCapabiliti~",
                        column: x => x.SupplierProcessCapabilityId,
                        principalTable: "SupplierProcessCapabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonUserAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthEmailContactPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsExternalUser = table.Column<bool>(type: "boolean", nullable: false),
                    InvitedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActivatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InvitedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonUserAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonUserAccounts_PartyContactPoints_AuthEmailContactPoint~",
                        column: x => x.AuthEmailContactPointId,
                        principalTable: "PartyContactPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonUserAccounts_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonUserAccounts_User_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTaskDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContactPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TriggerType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Priority = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TimeSemantics = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TimeZoneId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    NextRunAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PayloadJson = table.Column<string>(type: "text", nullable: true),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    LockedUntilUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTaskDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTaskDefinitions_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledTaskDefinitions_PartyContactPoints_ContactPointId",
                        column: x => x.ContactPointId,
                        principalTable: "PartyContactPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledTaskDefinitions_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposalNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LatestRevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IssuedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposals", x => x.Id);
                    table.UniqueConstraint("AK_BuyerProposals_Id_BuyerRfqId", x => new { x.Id, x.BuyerRfqId });
                    table.ForeignKey(
                        name: "FK_BuyerProposals_BusinessRelationships_BuyerBusinessRelations~",
                        column: x => x.BuyerBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposals_BuyerRfqs_BuyerRfqId_BuyerBusinessRelationsh~",
                        columns: x => new { x.BuyerRfqId, x.BuyerBusinessRelationshipId },
                        principalTable: "BuyerRfqs",
                        principalColumns: new[] { "Id", "BuyerBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Summary = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    BuyerOriginalText = table.Column<string>(type: "character varying(16000)", maxLength: 16000, nullable: true),
                    InternalInterpretation = table.Column<string>(type: "character varying(16000)", maxLength: 16000, nullable: true),
                    ResponseRequestedByUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReleasedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReleasedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupersededAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SnapshotHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqRevisions", x => x.Id);
                    table.UniqueConstraint("AK_BuyerRfqRevisions_Id_BuyerRfqId", x => new { x.Id, x.BuyerRfqId });
                    table.CheckConstraint("CK_BuyerRfqRevisions_ReleasedSnapshot", "\"Status\" NOT IN ('ReleasedForMatching', 'Superseded')\nOR\n(\n    \"ReleasedAtUtc\" IS NOT NULL\n    AND \"SnapshotHash\" IS NOT NULL\n    AND \"SnapshotJson\" IS NOT NULL\n)");
                    table.CheckConstraint("CK_BuyerRfqRevisions_SupersededDate", "\"SupersededAtUtc\" IS NULL\nOR\n(\n    \"ReleasedAtUtc\" IS NOT NULL\n    AND \"SupersededAtUtc\" >= \"ReleasedAtUtc\"\n)");
                    table.ForeignKey(
                        name: "FK_BuyerRfqRevisions_BuyerRfqs_BuyerRfqId",
                        column: x => x.BuyerRfqId,
                        principalTable: "BuyerRfqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerDemandStateHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerDemandId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromKnowledgeLevel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ToKnowledgeLevel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FromStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ToStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BuyerDemandEvidenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Confidence = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    ChangedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerDemandStateHistory", x => x.Id);
                    table.CheckConstraint("CK_BuyerDemandStateHistory_Confidence", "\"Confidence\" IS NULL\nOR (\"Confidence\" >= 0 AND \"Confidence\" <= 1)");
                    table.ForeignKey(
                        name: "FK_BuyerDemandStateHistory_BuyerDemandEvidence_BuyerDemandEvid~",
                        columns: x => new { x.BuyerDemandEvidenceId, x.BuyerDemandId },
                        principalTable: "BuyerDemandEvidence",
                        principalColumns: new[] { "Id", "BuyerDemandId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerDemandStateHistory_BuyerDemands_BuyerDemandId",
                        column: x => x.BuyerDemandId,
                        principalTable: "BuyerDemands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisclosurePackageItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisclosurePackageRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Classification = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SanitizedTextValue = table.Column<string>(type: "character varying(16000)", maxLength: 16000, nullable: true),
                    SanitizedJsonValue = table.Column<string>(type: "jsonb", nullable: true),
                    SourceEntityType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SourceEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourcePropertyName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisclosurePackageItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisclosurePackageItems_DisclosurePackageRevisions_Disclosur~",
                        column: x => x.DisclosurePackageRevisionId,
                        principalTable: "DisclosurePackageRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarrierBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    CarrierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BookingReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ExternalBookingId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequestedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ConfirmedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CostAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    CostCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    ServiceLevel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrierBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarrierBookings_BusinessRelationships_CarrierBusinessRelati~",
                        column: x => x.CarrierBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarrierBookings_Currencies_CostCurrencyId",
                        column: x => x.CostCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarrierBookings_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarrierBookings_ShipmentLegs_ShipmentLegId_ShipmentId",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarrierBookings_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StorageProvider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StorageKey = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    FileName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExternalDocumentId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssuedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VerifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentDocuments_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentDocuments_ShipmentLegs_ShipmentLegId_ShipmentId",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentDocuments_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentMilestoneTypeId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PlannedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExternalReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentMilestones_ShipmentLegs_ShipmentLegId_ShipmentId",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentMilestones_ShipmentMilestoneTypes_ShipmentMilestone~",
                        column: x => x.ShipmentMilestoneTypeId,
                        principalTable: "ShipmentMilestoneTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentMilestones_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentTrackingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: true),
                    EventCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExternalEventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTrackingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentTrackingEvents_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentTrackingEvents_GeographicLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentTrackingEvents_ShipmentLegs_ShipmentLegId_ShipmentId",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentTrackingEvents_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLotLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentInventoryLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildInventoryLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    InputQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: true),
                    YieldPercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    MaterialTransformationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLotLinks", x => x.Id);
                    table.CheckConstraint("CK_InventoryLotLinks_DifferentLots", "\"ParentInventoryLotId\" <> \"ChildInventoryLotId\"");
                    table.CheckConstraint("CK_InventoryLotLinks_Yield", "\"YieldPercent\" IS NULL\nOR (\"YieldPercent\" >= 0 AND \"YieldPercent\" <= 100)");
                    table.ForeignKey(
                        name: "FK_InventoryLotLinks_InventoryLots_ChildInventoryLotId",
                        column: x => x.ChildInventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLotLinks_InventoryLots_ParentInventoryLotId",
                        column: x => x.ParentInventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLotLinks_MaterialTransformations_MaterialTransform~",
                        column: x => x.MaterialTransformationId,
                        principalTable: "MaterialTransformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLotLinks_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLotOrigins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeographicLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    VerificationStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Proportion = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLotOrigins", x => x.Id);
                    table.CheckConstraint("CK_InventoryLotOrigins_Proportion", "\"Proportion\" IS NULL OR (\"Proportion\" >= 0 AND \"Proportion\" <= 1)");
                    table.ForeignKey(
                        name: "FK_InventoryLotOrigins_GeographicLocations_GeographicLocationId",
                        column: x => x.GeographicLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryLotOrigins_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentLotAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    ContainerReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SealReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentLotAllocations", x => x.Id);
                    table.CheckConstraint("CK_ShipmentLotAllocations_Quantity", "\"AllocatedQuantity\" > 0");
                    table.ForeignKey(
                        name: "FK_ShipmentLotAllocations_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLotAllocations_ShipmentLegs_ShipmentLegId_ShipmentId",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentLotAllocations_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentLotAllocations_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierOfferingAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    MinimumOrderQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    MinimumOrderUnitId = table.Column<int>(type: "integer", nullable: true),
                    AvailableFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AvailableToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    AvailabilityLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceReference = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LastConfirmedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierOfferingAvailabilities", x => x.Id);
                    table.UniqueConstraint("AK_SupplierOfferingAvailabilities_Id_SupplierMaterialOfferingId", x => new { x.Id, x.SupplierMaterialOfferingId });
                    table.CheckConstraint("CK_SupplierOfferingAvailabilities_MinimumOrder", "(\"MinimumOrderQuantity\" IS NULL AND \"MinimumOrderUnitId\" IS NULL)\nOR (\"MinimumOrderQuantity\" > 0 AND \"MinimumOrderUnitId\" IS NOT NULL)");
                    table.CheckConstraint("CK_SupplierOfferingAvailabilities_Quantities", "\"AvailableQuantity\" >= 0\nAND \"ReservedQuantity\" >= 0\nAND \"ReservedQuantity\" <= \"AvailableQuantity\"");
                    table.CheckConstraint("CK_SupplierOfferingAvailabilities_Window", "\"AvailableToUtc\" IS NULL\nOR \"AvailableToUtc\" >= \"AvailableFromUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierOfferingAvailabilities_GeographicLocations_Availabi~",
                        column: x => x.AvailabilityLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingAvailabilities_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingAvailabilities_SupplierMaterialOfferings_Su~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingAvailabilities_UnitsOfMeasure_MinimumOrderU~",
                        column: x => x.MinimumOrderUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingAvailabilities_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplyDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: false),
                    SubjectBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReferenceEntryId = table.Column<Guid>(type: "uuid", nullable: true),
                    StorageProvider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StorageKey = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    FileName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExternalDocumentId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssuedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VerifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyDocuments", x => x.Id);
                    table.CheckConstraint("CK_SupplyDocuments_Subject", "num_nonnulls(\"SubjectBusinessRelationshipId\", \"SupplierMaterialOfferingId\",\n    \"SupplierProcessCapabilityId\", \"InventoryLotId\", \"ShipmentId\") >= 1");
                    table.CheckConstraint("CK_SupplyDocuments_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_BusinessRelationships_SubjectBusinessRelati~",
                        column: x => x.SubjectBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_ReferenceEntries_ReferenceEntryId",
                        column: x => x.ReferenceEntryId,
                        principalTable: "ReferenceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_SupplierMaterialOfferings_SupplierMaterialO~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyDocuments_SupplierProcessCapabilities_SupplierProcess~",
                        column: x => x.SupplierProcessCapabilityId,
                        principalTable: "SupplierProcessCapabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContactPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TriggerType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Priority = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    DefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RunAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PayloadJson = table.Column<string>(type: "text", nullable: true),
                    LockedUntilUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    LastErrorAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    HangfireJobId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTasks_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledTasks_PartyContactPoints_ContactPointId",
                        column: x => x.ContactPointId,
                        principalTable: "PartyContactPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledTasks_ScheduledTaskDefinitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "ScheduledTaskDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduledTasks_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposalRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CommercialTerms = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    BuyerFacingNotes = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    DisclosurePackageRevisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IssuedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IssuedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    SnapshotHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposalRevisions", x => x.Id);
                    table.CheckConstraint("CK_BuyerProposalRevisions_Snapshot", "(\n    \"Status\" = 'Draft'\n    AND \"IssuedAtUtc\" IS NULL\n    AND \"SnapshotJson\" IS NULL\n    AND \"SnapshotHash\" IS NULL\n)\nOR\n(\n    \"Status\" <> 'Draft'\n    AND \"IssuedAtUtc\" IS NOT NULL\n    AND \"SnapshotJson\" IS NOT NULL\n    AND \"SnapshotHash\" IS NOT NULL\n)");
                    table.CheckConstraint("CK_BuyerProposalRevisions_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_BuyerProposalRevisions_BuyerProposals_BuyerProposalId_Buyer~",
                        columns: x => new { x.BuyerProposalId, x.BuyerRfqId },
                        principalTable: "BuyerProposals",
                        principalColumns: new[] { "Id", "BuyerRfqId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerProposalRevisions_BuyerRfqRevisions_BuyerRfqRevisionId~",
                        columns: x => new { x.BuyerRfqRevisionId, x.BuyerRfqId },
                        principalTable: "BuyerRfqRevisions",
                        principalColumns: new[] { "Id", "BuyerRfqId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalRevisions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalRevisions_DisclosurePackageRevisions_Disclosur~",
                        column: x => x.DisclosurePackageRevisionId,
                        principalTable: "DisclosurePackageRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryNumber = table.Column<int>(type: "integer", nullable: false),
                    DestinationAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedDeliveryFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RequestedDeliveryToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    NamedPlace = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    MinimumTemperatureCelsius = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    MaximumTemperatureCelsius = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    PackagingNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    DeliveryNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqDeliveries", x => x.Id);
                    table.UniqueConstraint("AK_BuyerRfqDeliveries_Id_BuyerRfqRevisionId", x => new { x.Id, x.BuyerRfqRevisionId });
                    table.CheckConstraint("CK_BuyerRfqDeliveries_Temperature", "\"MinimumTemperatureCelsius\" IS NULL\nOR \"MaximumTemperatureCelsius\" IS NULL\nOR \"MaximumTemperatureCelsius\" >= \"MinimumTemperatureCelsius\"");
                    table.CheckConstraint("CK_BuyerRfqDeliveries_Window", "\"RequestedDeliveryToUtc\" >= \"RequestedDeliveryFromUtc\"");
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveries_Addresses_DestinationAddressId",
                        column: x => x.DestinationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveries_BuyerRfqRevisions_BuyerRfqRevisionId",
                        column: x => x.BuyerRfqRevisionId,
                        principalTable: "BuyerRfqRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveries_GeographicLocations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveries_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerDemandScopeId = table.Column<Guid>(type: "uuid", nullable: true),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    BuyerLineReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BuyerOriginalText = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    IntendedUseCharacteristicId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequiredQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    LowerQuantityTolerancePercent = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    UpperQuantityTolerancePercent = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    AllowPartialFulfillment = table.Column<bool>(type: "boolean", nullable: false),
                    AllowMultipleSuppliers = table.Column<bool>(type: "boolean", nullable: false),
                    AllowMultipleOrigins = table.Column<bool>(type: "boolean", nullable: false),
                    AllowMultipleLots = table.Column<bool>(type: "boolean", nullable: false),
                    RequireSingleSupplier = table.Column<bool>(type: "boolean", nullable: false),
                    RequireSingleOrigin = table.Column<bool>(type: "boolean", nullable: false),
                    RequireSingleLot = table.Column<bool>(type: "boolean", nullable: false),
                    RequireUniformCertification = table.Column<bool>(type: "boolean", nullable: false),
                    RequireUniformSpecification = table.Column<bool>(type: "boolean", nullable: false),
                    MaximumSupplierCount = table.Column<int>(type: "integer", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqLines", x => x.Id);
                    table.UniqueConstraint("AK_BuyerRfqLines_Id_BuyerRfqRevisionId", x => new { x.Id, x.BuyerRfqRevisionId });
                    table.CheckConstraint("CK_BuyerRfqLines_MaximumSupplierCount", "\"MaximumSupplierCount\" IS NULL\nOR \"MaximumSupplierCount\" > 0");
                    table.CheckConstraint("CK_BuyerRfqLines_PoolingRules", "NOT (\"AllowMultipleSuppliers\" AND \"RequireSingleSupplier\")\nAND NOT (\"AllowMultipleOrigins\" AND \"RequireSingleOrigin\")\nAND NOT (\"AllowMultipleLots\" AND \"RequireSingleLot\")");
                    table.CheckConstraint("CK_BuyerRfqLines_RequiredQuantity", "\"RequiredQuantity\" > 0");
                    table.CheckConstraint("CK_BuyerRfqLines_Tolerances", "(\"LowerQuantityTolerancePercent\" IS NULL\n    OR (\"LowerQuantityTolerancePercent\" >= 0 AND \"LowerQuantityTolerancePercent\" <= 100))\nAND (\"UpperQuantityTolerancePercent\" IS NULL\n    OR (\"UpperQuantityTolerancePercent\" >= 0 AND \"UpperQuantityTolerancePercent\" <= 100))");
                    table.ForeignKey(
                        name: "FK_BuyerRfqLines_BuyerDemandScopes_BuyerDemandScopeId",
                        column: x => x.BuyerDemandScopeId,
                        principalTable: "BuyerDemandScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLines_BuyerRfqRevisions_BuyerRfqRevisionId",
                        column: x => x.BuyerRfqRevisionId,
                        principalTable: "BuyerRfqRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLines_Characteristics_IntendedUseCharacteristicId",
                        column: x => x.IntendedUseCharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLines_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLines_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchingRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingRuleSetVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    QueuedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TriggerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InputSnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    InputSnapshotHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingRuns", x => x.Id);
                    table.UniqueConstraint("AK_MatchingRuns_Id_BuyerRfqRevisionId", x => new { x.Id, x.BuyerRfqRevisionId });
                    table.CheckConstraint("CK_MatchingRuns_Completion", "\"CompletedAtUtc\" IS NULL\nOR\n(\n    \"StartedAtUtc\" IS NOT NULL\n    AND \"CompletedAtUtc\" >= \"StartedAtUtc\"\n)");
                    table.CheckConstraint("CK_MatchingRuns_InputSnapshot", "\"InputSnapshotJson\" IS NOT NULL\nAND \"InputSnapshotHash\" IS NOT NULL");
                    table.CheckConstraint("CK_MatchingRuns_Started", "\"StartedAtUtc\" IS NULL\nOR \"StartedAtUtc\" >= \"QueuedAtUtc\"");
                    table.CheckConstraint("CK_MatchingRuns_StatusTimestamps", "(\"Status\" = 'Queued' AND \"StartedAtUtc\" IS NULL AND \"CompletedAtUtc\" IS NULL)\nOR\n(\"Status\" = 'Running' AND \"StartedAtUtc\" IS NOT NULL AND \"CompletedAtUtc\" IS NULL)\nOR\n(\"Status\" IN ('Completed', 'Failed') AND \"StartedAtUtc\" IS NOT NULL AND \"CompletedAtUtc\" IS NOT NULL)\nOR\n(\"Status\" = 'Cancelled' AND \"CompletedAtUtc\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_MatchingRuns_BuyerRfqRevisions_BuyerRfqRevisionId",
                        column: x => x.BuyerRfqRevisionId,
                        principalTable: "BuyerRfqRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchingRuns_MatchingRuleSetVersions_MatchingRuleSetVersion~",
                        column: x => x.MatchingRuleSetVersionId,
                        principalTable: "MatchingRuleSetVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierQuotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuoteNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LatestRevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    SupplierReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ReceivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuotes", x => x.Id);
                    table.UniqueConstraint("AK_SupplierQuotes_Id_SupplierBusinessRelationshipId", x => new { x.Id, x.SupplierBusinessRelationshipId });
                    table.ForeignKey(
                        name: "FK_SupplierQuotes_BusinessRelationships_SupplierBusinessRelati~",
                        column: x => x.SupplierBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotes_BuyerRfqRevisions_BuyerRfqRevisionId",
                        column: x => x.BuyerRfqRevisionId,
                        principalTable: "BuyerRfqRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomsReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommodityCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IssuedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClearedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ShipmentDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomsReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomsReferences_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomsReferences_ShipmentDocuments_ShipmentDocumentId",
                        column: x => x.ShipmentDocumentId,
                        principalTable: "ShipmentDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomsReferences_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierOfferingSpecifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    ConstituentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: true),
                    Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MinimumValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    MaximumValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    TargetValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    TextValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    Basis = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AnalysisMethod = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    VerificationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SupplyDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierOfferingSpecifications", x => x.Id);
                    table.CheckConstraint("CK_SupplierOfferingSpecifications_Range", "\"MinimumValue\" IS NULL\nOR \"MaximumValue\" IS NULL\nOR \"MaximumValue\" >= \"MinimumValue\"");
                    table.CheckConstraint("CK_SupplierOfferingSpecifications_Target", "(\"Kind\" <> 'Constituent' OR \"ConstituentId\" IS NOT NULL)\nAND (\"Kind\" <> 'Characteristic' OR \"CharacteristicId\" IS NOT NULL)");
                    table.CheckConstraint("CK_SupplierOfferingSpecifications_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierOfferingSpecifications_Characteristics_Characterist~",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingSpecifications_Constituents_ConstituentId",
                        column: x => x.ConstituentId,
                        principalTable: "Constituents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingSpecifications_SupplierMaterialOfferings_Su~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingSpecifications_SupplyDocuments_SupplyDocume~",
                        column: x => x.SupplyDocumentId,
                        principalTable: "SupplyDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierOfferingSpecifications_UnitsOfMeasure_UnitOfMeasure~",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplyAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplyDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    LaboratoryBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AnalysisNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SampleIdentifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LaboratoryName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LaboratoryAccredited = table.Column<bool>(type: "boolean", nullable: false),
                    AccreditationReference = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SampledAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReceivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReportedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VerifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyAnalyses", x => x.Id);
                    table.CheckConstraint("CK_SupplyAnalyses_Subject", "num_nonnulls(\"SupplierMaterialOfferingId\", \"InventoryLotId\") >= 1");
                    table.ForeignKey(
                        name: "FK_SupplyAnalyses_BusinessRelationships_LaboratoryBusinessRela~",
                        column: x => x.LaboratoryBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAnalyses_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAnalyses_SupplierMaterialOfferings_SupplierMaterialOf~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAnalyses_SupplyDocuments_SupplyDocumentId",
                        column: x => x.SupplyDocumentId,
                        principalTable: "SupplyDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplyCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CertificationSchemeId = table.Column<int>(type: "integer", nullable: false),
                    CertificationSchemeJurisdictionId = table.Column<int>(type: "integer", nullable: true),
                    CertifiedBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CertifiedSiteLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Scope = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CertificateNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CertificateHolderName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssuerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IssuerBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    CertifiedScopeText = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SupplyDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    VerificationNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyCertifications", x => x.Id);
                    table.CheckConstraint("CK_SupplyCertifications_Subject", "num_nonnulls(\"CertifiedBusinessRelationshipId\", \"SupplierMaterialOfferingId\",\n    \"SupplierProcessCapabilityId\", \"InventoryLotId\", \"ShipmentId\") = 1");
                    table.CheckConstraint("CK_SupplyCertifications_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_BusinessRelationships_CertifiedBusines~",
                        column: x => x.CertifiedBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_BusinessRelationships_IssuerBusinessRe~",
                        column: x => x.IssuerBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_CertificationSchemeJurisdictions_Certi~",
                        columns: x => new { x.CertificationSchemeJurisdictionId, x.CertificationSchemeId },
                        principalTable: "CertificationSchemeJurisdictions",
                        principalColumns: new[] { "Id", "CertificationSchemeId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_CertificationSchemes_CertificationSche~",
                        column: x => x.CertificationSchemeId,
                        principalTable: "CertificationSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_GeographicLocations_CertifiedSiteLocat~",
                        column: x => x.CertifiedSiteLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_SupplierMaterialOfferings_SupplierMate~",
                        column: x => x.SupplierMaterialOfferingId,
                        principalTable: "SupplierMaterialOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_SupplierProcessCapabilities_SupplierPr~",
                        column: x => x.SupplierProcessCapabilityId,
                        principalTable: "SupplierProcessCapabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyCertifications_SupplyDocuments_SupplyDocumentId",
                        column: x => x.SupplyDocumentId,
                        principalTable: "SupplyDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqScheduledTaskLinks",
                columns: table => new
                {
                    BuyerRfqId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqScheduledTaskLinks", x => new { x.BuyerRfqId, x.ScheduledTaskId });
                    table.ForeignKey(
                        name: "FK_BuyerRfqScheduledTaskLinks_BuyerRfqs_BuyerRfqId",
                        column: x => x.BuyerRfqId,
                        principalTable: "BuyerRfqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerRfqScheduledTaskLinks_ScheduledTasks_ScheduledTaskId",
                        column: x => x.ScheduledTaskId,
                        principalTable: "ScheduledTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmActivityScheduledTaskLinks",
                columns: table => new
                {
                    CrmActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmActivityScheduledTaskLinks", x => new { x.CrmActivityId, x.ScheduledTaskId });
                    table.ForeignKey(
                        name: "FK_CrmActivityScheduledTaskLinks_CrmActivities_CrmActivityId",
                        column: x => x.CrmActivityId,
                        principalTable: "CrmActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrmActivityScheduledTaskLinks_ScheduledTasks_ScheduledTaskId",
                        column: x => x.ScheduledTaskId,
                        principalTable: "ScheduledTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailOutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplyToUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FromEmailSnapshot = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    FromNameSnapshot = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ReplyToEmailSnapshot = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    ReplyToNameSnapshot = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContentMode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Provider = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProviderTemplateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    VariablesJson = table.Column<string>(type: "text", nullable: true),
                    PersonalizationsJson = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    BodyHtml = table.Column<string>(type: "text", nullable: false),
                    BodyText = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ScheduledTaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutboxMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailOutboxMessages_ScheduledTasks_ScheduledTaskId",
                        column: x => x.ScheduledTaskId,
                        principalTable: "ScheduledTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposalRevisionScheduledTaskLinks",
                columns: table => new
                {
                    BuyerProposalRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposalRevisionScheduledTaskLinks", x => new { x.BuyerProposalRevisionId, x.ScheduledTaskId });
                    table.ForeignKey(
                        name: "FK_BuyerProposalRevisionScheduledTaskLinks_BuyerProposalRevisi~",
                        column: x => x.BuyerProposalRevisionId,
                        principalTable: "BuyerProposalRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerProposalRevisionScheduledTaskLinks_ScheduledTasks_Sche~",
                        column: x => x.ScheduledTaskId,
                        principalTable: "ScheduledTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqDeliveryLineAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqDeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqDeliveryLineAllocations", x => x.Id);
                    table.CheckConstraint("CK_BuyerRfqDeliveryLineAllocations_Quantity", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveryLineAllocations_BuyerRfqDeliveries_BuyerRfq~",
                        columns: x => new { x.BuyerRfqDeliveryId, x.BuyerRfqRevisionId },
                        principalTable: "BuyerRfqDeliveries",
                        principalColumns: new[] { "Id", "BuyerRfqRevisionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveryLineAllocations_BuyerRfqLines_BuyerRfqLineI~",
                        columns: x => new { x.BuyerRfqLineId, x.BuyerRfqRevisionId },
                        principalTable: "BuyerRfqLines",
                        principalColumns: new[] { "Id", "BuyerRfqRevisionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqDeliveryLineAllocations_UnitsOfMeasure_QuantityUnit~",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqLineMaterialOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionKind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BiologicalSourceClaimId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerText = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AlternativeGroupKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MinimumMappingConfidence = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqLineMaterialOptions", x => x.Id);
                    table.CheckConstraint("CK_BuyerRfqLineMaterialOptions_Target", "num_nonnulls(\"MaterialId\", \"MaterialTypeId\", \"BiologicalSourceId\", \"BiologicalSourceClaimId\",\n    \"BuyerText\") >= 1");
                    table.ForeignKey(
                        name: "FK_BuyerRfqLineMaterialOptions_BiologicalSources_BiologicalSou~",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLineMaterialOptions_BusinessRelationshipBiologicalS~",
                        column: x => x.BiologicalSourceClaimId,
                        principalTable: "BusinessRelationshipBiologicalSourceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLineMaterialOptions_BuyerRfqLines_BuyerRfqLineId",
                        column: x => x.BuyerRfqLineId,
                        principalTable: "BuyerRfqLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLineMaterialOptions_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqLineMaterialOptions_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerRfqRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Level = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    AlternativeGroupKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    BuyerOriginalText = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    RequirementType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BiologicalSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerRfqBiologicalSourceRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CertificationSchemeId = table.Column<int>(type: "integer", nullable: true),
                    BuyerRfqCertificationRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RequiredScope = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CertificationSchemeJurisdictionId = table.Column<int>(type: "integer", nullable: true),
                    MustBeValidAtDelivery = table.Column<bool>(type: "boolean", nullable: true),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerRfqCharacteristicRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExpectedTextValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ExpectedNumericValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    BuyerRfqCharacteristicRequirement_UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    BuyerRfqCommercialRequirement_Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BuyerRfqCommercialRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BuyerRfqCommercialRequirement_NumericValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    BuyerRfqCommercialRequirement_TextValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    BuyerRfqCommercialRequirement_UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    CurrencyId = table.Column<int>(type: "integer", nullable: true),
                    ConstituentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerRfqConstituentRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    MinimumValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    MaximumValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    TargetValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    BuyerRfqConstituentRequirement_UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    Basis = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AnalysisMethod = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MustBeVerifiedForEveryLot = table.Column<bool>(type: "boolean", nullable: true),
                    AccreditedLaboratoryRequired = table.Column<bool>(type: "boolean", nullable: true),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: true),
                    BuyerRfqDocumentRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    MustBeLotSpecific = table.Column<bool>(type: "boolean", nullable: true),
                    MustBeIssuedByAccreditedParty = table.Column<bool>(type: "boolean", nullable: true),
                    MaximumAgeDays = table.Column<int>(type: "integer", nullable: true),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BuyerRfqLogisticsRequirement_Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    NumericValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    TextValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    MatchMode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AllowSubstitutes = table.Column<bool>(type: "boolean", nullable: true),
                    GeographicLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginRole = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Operator = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProcessTypeId = table.Column<int>(type: "integer", nullable: true),
                    Mode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProcessParametersJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerRfqRequirements", x => x.Id);
                    table.CheckConstraint("CK_BuyerRfqRequirements_Weight", "\"Weight\" >= 0");
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_BiologicalSources_BiologicalSourceId",
                        column: x => x.BiologicalSourceId,
                        principalTable: "BiologicalSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_BuyerRfqLines_BuyerRfqLineId_BuyerRfqR~",
                        columns: x => new { x.BuyerRfqLineId, x.BuyerRfqRevisionId },
                        principalTable: "BuyerRfqLines",
                        principalColumns: new[] { "Id", "BuyerRfqRevisionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_BuyerRfqRevisions_BuyerRfqRevisionId",
                        column: x => x.BuyerRfqRevisionId,
                        principalTable: "BuyerRfqRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_CertificationSchemeJurisdictions_Certi~",
                        columns: x => new { x.CertificationSchemeJurisdictionId, x.CertificationSchemeId },
                        principalTable: "CertificationSchemeJurisdictions",
                        principalColumns: new[] { "Id", "CertificationSchemeId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_CertificationSchemes_CertificationSche~",
                        column: x => x.CertificationSchemeId,
                        principalTable: "CertificationSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_Characteristics_CharacteristicId",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_Constituents_ConstituentId",
                        column: x => x.ConstituentId,
                        principalTable: "Constituents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_GeographicLocations_GeographicLocation~",
                        column: x => x.GeographicLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_ProcessTypes_ProcessTypeId",
                        column: x => x.ProcessTypeId,
                        principalTable: "ProcessTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_UnitsOfMeasure_BuyerRfqCharacteristicR~",
                        column: x => x.BuyerRfqCharacteristicRequirement_UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_UnitsOfMeasure_BuyerRfqCommercialRequi~",
                        column: x => x.BuyerRfqCommercialRequirement_UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_UnitsOfMeasure_BuyerRfqConstituentRequ~",
                        column: x => x.BuyerRfqConstituentRequirement_UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerRfqRequirements_UnitsOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchingRunLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    InputSnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingRunLines", x => x.Id);
                    table.UniqueConstraint("AK_MatchingRunLines_Id_MatchingRunId", x => new { x.Id, x.MatchingRunId });
                    table.CheckConstraint("CK_MatchingRunLines_RequiredQuantity", "\"RequiredQuantity\" > 0");
                    table.ForeignKey(
                        name: "FK_MatchingRunLines_BuyerRfqLines_BuyerRfqLineId_BuyerRfqRevis~",
                        columns: x => new { x.BuyerRfqLineId, x.BuyerRfqRevisionId },
                        principalTable: "BuyerRfqLines",
                        principalColumns: new[] { "Id", "BuyerRfqRevisionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchingRunLines_MatchingRuns_MatchingRunId_BuyerRfqRevisio~",
                        columns: x => new { x.MatchingRunId, x.BuyerRfqRevisionId },
                        principalTable: "MatchingRuns",
                        principalColumns: new[] { "Id", "BuyerRfqRevisionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchingRunLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplySolutionCandidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Outcome = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VerificationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TotalScore = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: false),
                    CoveragePercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    EstimatedPurchaseCost = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    EstimatedLogisticsCost = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    EstimatedProcessingCost = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    EstimatedTotalCost = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    EstimatedCostCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    EstimatedLeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    ContainsHardFail = table.Column<bool>(type: "boolean", nullable: false),
                    SnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    SnapshotHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplySolutionCandidates", x => x.Id);
                    table.UniqueConstraint("AK_SupplySolutionCandidates_Id_MatchingRunId", x => new { x.Id, x.MatchingRunId });
                    table.CheckConstraint("CK_SupplySolutionCandidates_Coverage", "\"CoveragePercent\" >= 0\nAND \"CoveragePercent\" <= 100");
                    table.CheckConstraint("CK_SupplySolutionCandidates_Score", "\"TotalScore\" >= 0");
                    table.CheckConstraint("CK_SupplySolutionCandidates_Snapshot", "(\n    \"SnapshotJson\" IS NULL\n    AND \"SnapshotHash\" IS NULL\n)\nOR\n(\n    \"SnapshotJson\" IS NOT NULL\n    AND \"SnapshotHash\" IS NOT NULL\n)");
                    table.ForeignKey(
                        name: "FK_SupplySolutionCandidates_Currencies_EstimatedCostCurrencyId",
                        column: x => x.EstimatedCostCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionCandidates_MatchingRuns_MatchingRunId",
                        column: x => x.MatchingRunId,
                        principalTable: "MatchingRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplierQuoteRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierQuoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    NamedPlace = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PaymentTerms = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    ValidFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReceivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OriginalDocumentReference = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    SnapshotHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuoteRevisions", x => x.Id);
                    table.UniqueConstraint("AK_SupplierQuoteRevisions_Id_SupplierBusinessRelationshipId", x => new { x.Id, x.SupplierBusinessRelationshipId });
                    table.CheckConstraint("CK_SupplierQuoteRevisions_ReceivedSnapshot", "\"Status\" = 'Draft'\nOR\n(\n    \"ReceivedAtUtc\" IS NOT NULL\n    AND \"SnapshotJson\" IS NOT NULL\n    AND \"SnapshotHash\" IS NOT NULL\n)");
                    table.CheckConstraint("CK_SupplierQuoteRevisions_Snapshot", "(\n    \"SnapshotJson\" IS NULL\n    AND \"SnapshotHash\" IS NULL\n)\nOR\n(\n    \"SnapshotJson\" IS NOT NULL\n    AND \"SnapshotHash\" IS NOT NULL\n)");
                    table.CheckConstraint("CK_SupplierQuoteRevisions_Validity", "\"ValidToUtc\" IS NULL\nOR \"ValidFromUtc\" IS NULL\nOR \"ValidToUtc\" >= \"ValidFromUtc\"");
                    table.ForeignKey(
                        name: "FK_SupplierQuoteRevisions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteRevisions_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteRevisions_SupplierQuotes_SupplierQuoteId_Suppl~",
                        columns: x => new { x.SupplierQuoteId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierQuotes",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplyAnalysisResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplyAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConstituentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacteristicId = table.Column<Guid>(type: "uuid", nullable: true),
                    AnalyteName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Qualifier = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NumericValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    RangeMaximumValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    TextValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    Basis = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DetectionLimit = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    AnalysisMethod = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    VerificationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyAnalysisResults", x => x.Id);
                    table.CheckConstraint("CK_SupplyAnalysisResults_Analyte", "num_nonnulls(\"ConstituentId\", \"CharacteristicId\") <= 1\nAND\nnum_nonnulls(\"ConstituentId\", \"CharacteristicId\", \"AnalyteName\") >= 1");
                    table.ForeignKey(
                        name: "FK_SupplyAnalysisResults_Characteristics_CharacteristicId",
                        column: x => x.CharacteristicId,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAnalysisResults_Constituents_ConstituentId",
                        column: x => x.ConstituentId,
                        principalTable: "Constituents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAnalysisResults_SupplyAnalyses_SupplyAnalysisId",
                        column: x => x.SupplyAnalysisId,
                        principalTable: "SupplyAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyAnalysisResults_UnitsOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailOutboxRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    EmailSnapshot = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    NameSnapshot = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PersonalizationJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutboxRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailOutboxRecipients_EmailOutboxMessages_OutboxMessageId",
                        column: x => x.OutboxMessageId,
                        principalTable: "EmailOutboxMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposalOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TotalNetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TotalTaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    TotalGrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    EstimatedLeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    BuyerFacingSummary = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    InternalMarginSnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposalOptions", x => x.Id);
                    table.UniqueConstraint("AK_BuyerProposalOptions_Id_BuyerProposalRevisionId", x => new { x.Id, x.BuyerProposalRevisionId });
                    table.CheckConstraint("CK_BuyerProposalOptions_Amounts", "\"TotalNetAmount\" >= 0\nAND \"TotalGrossAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_BuyerProposalOptions_BuyerProposalRevisions_BuyerProposalRe~",
                        column: x => x.BuyerProposalRevisionId,
                        principalTable: "BuyerProposalRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerProposalOptions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalOptions_SupplySolutionCandidates_SupplySolutio~",
                        column: x => x.SupplySolutionCandidateId,
                        principalTable: "SupplySolutionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchingModerationDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Decision = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DecidedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DecidedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsManualOverride = table.Column<bool>(type: "boolean", nullable: false),
                    OverriddenOutcome = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OverriddenScore = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: true),
                    Reason = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingModerationDecisions", x => x.Id);
                    table.CheckConstraint("CK_MatchingModerationDecisions_OverriddenScore", "\"OverriddenScore\" IS NULL\nOR \"OverriddenScore\" >= 0");
                    table.CheckConstraint("CK_MatchingModerationDecisions_Override", "(\n    NOT \"IsManualOverride\"\n    AND \"OverriddenOutcome\" IS NULL\n    AND \"OverriddenScore\" IS NULL\n)\nOR\n(\n    \"IsManualOverride\"\n    AND\n    (\n        \"OverriddenOutcome\" IS NOT NULL\n        OR \"OverriddenScore\" IS NOT NULL\n    )\n)");
                    table.ForeignKey(
                        name: "FK_MatchingModerationDecisions_SupplySolutionCandidates_Supply~",
                        column: x => x.SupplySolutionCandidateId,
                        principalTable: "SupplySolutionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplySolutionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingRunLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    CoveredQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    ShortageQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    IsFullyCovered = table.Column<bool>(type: "boolean", nullable: false),
                    CoveragePercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplySolutionLines", x => x.Id);
                    table.UniqueConstraint("AK_SupplySolutionLines_Id_SupplySolutionCandidateId", x => new { x.Id, x.SupplySolutionCandidateId });
                    table.CheckConstraint("CK_SupplySolutionLines_Quantities", "\"RequiredQuantity\" > 0\nAND \"CoveredQuantity\" >= 0\nAND \"ShortageQuantity\" >= 0\nAND \"CoveredQuantity\" + \"ShortageQuantity\" = \"RequiredQuantity\"\nAND \"CoveragePercent\" >= 0\nAND \"CoveragePercent\" <= 100\nAND (NOT \"IsFullyCovered\" OR \"ShortageQuantity\" = 0)");
                    table.ForeignKey(
                        name: "FK_SupplySolutionLines_MatchingRunLines_MatchingRunLineId_Matc~",
                        columns: x => new { x.MatchingRunLineId, x.MatchingRunId },
                        principalTable: "MatchingRunLines",
                        principalColumns: new[] { "Id", "MatchingRunId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionLines_SupplySolutionCandidates_SupplySolution~",
                        columns: x => new { x.SupplySolutionCandidateId, x.MatchingRunId },
                        principalTable: "SupplySolutionCandidates",
                        principalColumns: new[] { "Id", "MatchingRunId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplySolutionLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierQuoteRevisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    SupplierReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    NamedPlace = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PaymentTerms = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    OrderDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SentAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedAvailabilityFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedAvailabilityToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPurchaseOrders", x => x.Id);
                    table.UniqueConstraint("AK_SupplierPurchaseOrders_Id_SupplierBusinessRelationshipId", x => new { x.Id, x.SupplierBusinessRelationshipId });
                    table.CheckConstraint("CK_SupplierPurchaseOrders_Amounts", "\"NetAmount\" >= 0\nAND \"TaxAmount\" >= 0\nAND \"GrossAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrders_BusinessRelationships_SupplierBusine~",
                        column: x => x.SupplierBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrders_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrders_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrders_SupplierQuoteRevisions_SupplierQuote~",
                        columns: x => new { x.SupplierQuoteRevisionId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierQuoteRevisions",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposalAcceptances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BuyerAcceptanceReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AcceptedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPartialAcceptance = table.Column<bool>(type: "boolean", nullable: false),
                    InternallyApprovedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InternallyApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalApprovalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposalAcceptances", x => x.Id);
                    table.UniqueConstraint("AK_BuyerProposalAcceptances_Id_BuyerProposalOptionId", x => new { x.Id, x.BuyerProposalOptionId });
                    table.CheckConstraint("CK_BuyerProposalAcceptances_Approval", "(\n    \"Status\" = 'Approved'\n    AND \"InternallyApprovedAtUtc\" IS NOT NULL\n)\nOR\n(\n    \"Status\" <> 'Approved'\n    AND \"InternallyApprovedAtUtc\" IS NULL\n)");
                    table.ForeignKey(
                        name: "FK_BuyerProposalAcceptances_BuyerProposalOptions_BuyerProposal~",
                        columns: x => new { x.BuyerProposalOptionId, x.BuyerProposalRevisionId },
                        principalTable: "BuyerProposalOptions",
                        principalColumns: new[] { "Id", "BuyerProposalRevisionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalAcceptances_BuyerProposalRevisions_BuyerPropos~",
                        column: x => x.BuyerProposalRevisionId,
                        principalTable: "BuyerProposalRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposalLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    EstimatedDeliveryFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EstimatedDeliveryToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BuyerFacingNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposalLines", x => x.Id);
                    table.UniqueConstraint("AK_BuyerProposalLines_Id_BuyerProposalOptionId", x => new { x.Id, x.BuyerProposalOptionId });
                    table.CheckConstraint("CK_BuyerProposalLines_DeliveryWindow", "\"EstimatedDeliveryToUtc\" IS NULL\nOR \"EstimatedDeliveryFromUtc\" IS NULL\nOR \"EstimatedDeliveryToUtc\" >= \"EstimatedDeliveryFromUtc\"");
                    table.CheckConstraint("CK_BuyerProposalLines_Values", "\"Quantity\" > 0\nAND \"UnitPrice\" >= 0\nAND \"NetAmount\" >= 0\nAND \"GrossAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_BuyerProposalLines_BuyerProposalOptions_BuyerProposalOption~",
                        column: x => x.BuyerProposalOptionId,
                        principalTable: "BuyerProposalOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerProposalLines_BuyerRfqLines_BuyerRfqLineId",
                        column: x => x.BuyerRfqLineId,
                        principalTable: "BuyerRfqLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalLines_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalLines_SupplySolutionLines_SupplySolutionLineId",
                        column: x => x.SupplySolutionLineId,
                        principalTable: "SupplySolutionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplySolutionSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    ProviderBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProcessTypeId = table.Column<int>(type: "integer", nullable: true),
                    InputMaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutputMaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpectedInputQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    InputQuantityUnitId = table.Column<int>(type: "integer", nullable: true),
                    ExpectedOutputQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    OutputQuantityUnitId = table.Column<int>(type: "integer", nullable: true),
                    ExpectedYieldPercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    EstimatedLeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    FromLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlannedStartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PlannedEndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ParametersJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplySolutionSteps", x => x.Id);
                    table.UniqueConstraint("AK_SupplySolutionSteps_Id_SupplySolutionCandidateId", x => new { x.Id, x.SupplySolutionCandidateId });
                    table.CheckConstraint("CK_SupplySolutionSteps_CapabilityProvider", "\"SupplierProcessCapabilityId\" IS NULL\nOR \"ProviderBusinessRelationshipId\" IS NOT NULL");
                    table.CheckConstraint("CK_SupplySolutionSteps_PlannedWindow", "\"PlannedEndUtc\" IS NULL\nOR \"PlannedStartUtc\" IS NULL\nOR \"PlannedEndUtc\" >= \"PlannedStartUtc\"");
                    table.CheckConstraint("CK_SupplySolutionSteps_Quantities", "((\"ExpectedInputQuantity\" IS NULL AND \"InputQuantityUnitId\" IS NULL)\n    OR (\"ExpectedInputQuantity\" >= 0 AND \"InputQuantityUnitId\" IS NOT NULL))\nAND ((\"ExpectedOutputQuantity\" IS NULL AND \"OutputQuantityUnitId\" IS NULL)\n    OR (\"ExpectedOutputQuantity\" >= 0 AND \"OutputQuantityUnitId\" IS NOT NULL))");
                    table.CheckConstraint("CK_SupplySolutionSteps_Yield", "\"ExpectedYieldPercent\" IS NULL\nOR (\"ExpectedYieldPercent\" >= 0 AND \"ExpectedYieldPercent\" <= 100)");
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_BusinessRelationships_ProviderBusinessR~",
                        column: x => x.ProviderBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_GeographicLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_GeographicLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_Materials_InputMaterialId",
                        column: x => x.InputMaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_Materials_OutputMaterialId",
                        column: x => x.OutputMaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_ProcessTypes_ProcessTypeId",
                        column: x => x.ProcessTypeId,
                        principalTable: "ProcessTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_SupplierProcessCapabilities_SupplierPro~",
                        columns: x => new { x.SupplierProcessCapabilityId, x.ProviderBusinessRelationshipId },
                        principalTable: "SupplierProcessCapabilities",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_SupplySolutionCandidates_SupplySolution~",
                        column: x => x.SupplySolutionCandidateId,
                        principalTable: "SupplySolutionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_SupplySolutionLines_SupplySolutionLineI~",
                        columns: x => new { x.SupplySolutionLineId, x.SupplySolutionCandidateId },
                        principalTable: "SupplySolutionLines",
                        principalColumns: new[] { "Id", "SupplySolutionCandidateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_UnitsOfMeasure_InputQuantityUnitId",
                        column: x => x.InputQuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionSteps_UnitsOfMeasure_OutputQuantityUnitId",
                        column: x => x.OutputQuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPurchaseOrderStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ToStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ChangedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPurchaseOrderStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderStatusHistory_SupplierPurchaseOrders_S~",
                        column: x => x.SupplierPurchaseOrderId,
                        principalTable: "SupplierPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerSalesOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalAcceptanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    BuyerPurchaseOrderReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    NamedPlace = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PaymentTerms = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    OrderDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedDeliveryFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedDeliveryToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerSalesOrders", x => x.Id);
                    table.UniqueConstraint("AK_BuyerSalesOrders_Id_BuyerProposalAcceptanceId", x => new { x.Id, x.BuyerProposalAcceptanceId });
                    table.CheckConstraint("CK_BuyerSalesOrders_Amounts", "\"NetAmount\" >= 0\nAND \"TaxAmount\" >= 0\nAND \"GrossAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrders_BusinessRelationships_BuyerBusinessRelatio~",
                        column: x => x.BuyerBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrders_BuyerProposalAcceptances_BuyerProposalAcce~",
                        column: x => x.BuyerProposalAcceptanceId,
                        principalTable: "BuyerProposalAcceptances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrders_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrders_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerProposalAcceptanceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalAcceptanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcceptedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    QuantityChanged = table.Column<bool>(type: "boolean", nullable: false),
                    BuyerNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerProposalAcceptanceLines", x => x.Id);
                    table.UniqueConstraint("AK_BuyerProposalAcceptanceLines_Id_BuyerProposalAcceptanceId", x => new { x.Id, x.BuyerProposalAcceptanceId });
                    table.CheckConstraint("CK_BuyerProposalAcceptanceLines_Quantity", "\"AcceptedQuantity\" > 0");
                    table.ForeignKey(
                        name: "FK_BuyerProposalAcceptanceLines_BuyerProposalAcceptances_Buyer~",
                        columns: x => new { x.BuyerProposalAcceptanceId, x.BuyerProposalOptionId },
                        principalTable: "BuyerProposalAcceptances",
                        principalColumns: new[] { "Id", "BuyerProposalOptionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerProposalAcceptanceLines_BuyerProposalLines_BuyerPropos~",
                        columns: x => new { x.BuyerProposalLineId, x.BuyerProposalOptionId },
                        principalTable: "BuyerProposalLines",
                        principalColumns: new[] { "Id", "BuyerProposalOptionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerProposalAcceptanceLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierQuoteLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierQuoteRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplySolutionStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    OfferedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    FixedFee = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    MinimumOrderQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    AvailableFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AvailableToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    NamedPlace = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PriceBasis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SupplierLineReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuoteLines", x => x.Id);
                    table.UniqueConstraint("AK_SupplierQuoteLines_Id_SupplierBusinessRelationshipId", x => new { x.Id, x.SupplierBusinessRelationshipId });
                    table.CheckConstraint("CK_SupplierQuoteLines_Availability", "\"AvailableToUtc\" IS NULL\nOR \"AvailableFromUtc\" IS NULL\nOR \"AvailableToUtc\" >= \"AvailableFromUtc\"");
                    table.CheckConstraint("CK_SupplierQuoteLines_Target", "num_nonnulls(\"SupplierMaterialOfferingId\", \"SupplierProcessCapabilityId\",\n    \"SupplySolutionStepId\", \"InventoryLotId\") >= 1");
                    table.CheckConstraint("CK_SupplierQuoteLines_Values", "\"OfferedQuantity\" > 0\nAND \"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_BuyerRfqLines_BuyerRfqLineId",
                        column: x => x.BuyerRfqLineId,
                        principalTable: "BuyerRfqLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_InventoryLots_InventoryLotId_SupplierBus~",
                        columns: x => new { x.InventoryLotId, x.SupplierBusinessRelationshipId },
                        principalTable: "InventoryLots",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_SupplierMaterialOfferings_SupplierMateri~",
                        columns: x => new { x.SupplierMaterialOfferingId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierMaterialOfferings",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_SupplierProcessCapabilities_SupplierProc~",
                        columns: x => new { x.SupplierProcessCapabilityId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierProcessCapabilities",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_SupplierQuoteRevisions_SupplierQuoteRevi~",
                        columns: x => new { x.SupplierQuoteRevisionId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierQuoteRevisions",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_SupplySolutionSteps_SupplySolutionStepId",
                        column: x => x.SupplySolutionStepId,
                        principalTable: "SupplySolutionSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuoteLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplySolutionStepDependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    PredecessorStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuccessorStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplySolutionStepDependencies", x => x.Id);
                    table.CheckConstraint("CK_SupplySolutionStepDependencies_DifferentSteps", "\"PredecessorStepId\" <> \"SuccessorStepId\"");
                    table.ForeignKey(
                        name: "FK_SupplySolutionStepDependencies_SupplySolutionSteps_Predeces~",
                        columns: x => new { x.PredecessorStepId, x.SupplySolutionCandidateId },
                        principalTable: "SupplySolutionSteps",
                        principalColumns: new[] { "Id", "SupplySolutionCandidateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplySolutionStepDependencies_SupplySolutionSteps_Successo~",
                        columns: x => new { x.SuccessorStepId, x.SupplySolutionCandidateId },
                        principalTable: "SupplySolutionSteps",
                        principalColumns: new[] { "Id", "SupplySolutionCandidateId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerSalesOrderDeliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryNumber = table.Column<int>(type: "integer", nullable: false),
                    DestinationAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    NamedPlace = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PlannedDeliveryFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PlannedDeliveryToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActualDeliveryAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveryInstructions = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerSalesOrderDeliveries", x => x.Id);
                    table.UniqueConstraint("AK_BuyerSalesOrderDeliveries_Id_BuyerSalesOrderId", x => new { x.Id, x.BuyerSalesOrderId });
                    table.CheckConstraint("CK_BuyerSalesOrderDeliveries_Window", "\"PlannedDeliveryToUtc\" >= \"PlannedDeliveryFromUtc\"");
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveries_Addresses_DestinationAddressId",
                        column: x => x.DestinationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveries_BuyerSalesOrders_BuyerSalesOrderId",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveries_GeographicLocations_DestinationLo~",
                        column: x => x.DestinationLocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveries_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerSalesOrderStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ToStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ChangedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerSalesOrderStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderStatusHistory_BuyerSalesOrders_BuyerSalesOrd~",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Direction = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CounterpartyBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: true),
                    SystemOwnership = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SyncStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    InvoiceDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DueDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BookedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExternalInvoiceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ExternalInvoiceNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialInvoices", x => x.Id);
                    table.CheckConstraint("CK_CommercialInvoices_Amounts", "\"NetAmount\" >= 0\nAND \"TaxAmount\" >= 0\nAND \"GrossAmount\" >= 0\nAND \"PaidAmount\" >= 0\nAND \"OutstandingAmount\" >= 0");
                    table.CheckConstraint("CK_CommercialInvoices_OrderSide", "num_nonnulls(\"BuyerSalesOrderId\", \"SupplierPurchaseOrderId\") <= 1");
                    table.ForeignKey(
                        name: "FK_CommercialInvoices_BusinessRelationships_CounterpartyBusine~",
                        column: x => x.CounterpartyBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialInvoices_BuyerSalesOrders_BuyerSalesOrderId",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialInvoices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialInvoices_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialInvoices_SupplierPurchaseOrders_SupplierPurchaseO~",
                        column: x => x.SupplierPurchaseOrderId,
                        principalTable: "SupplierPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalConversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PrimaryChannel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BuyerRfqId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierQuoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerProposalId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastMessageAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InternalNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalConversations_BusinessRelationships_ExternalBusines~",
                        column: x => x.ExternalBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalConversations_BuyerProposals_BuyerProposalId",
                        column: x => x.BuyerProposalId,
                        principalTable: "BuyerProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalConversations_BuyerRfqs_BuyerRfqId",
                        column: x => x.BuyerRfqId,
                        principalTable: "BuyerRfqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalConversations_BuyerSalesOrders_BuyerSalesOrderId",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalConversations_SupplierPurchaseOrders_SupplierPurcha~",
                        column: x => x.SupplierPurchaseOrderId,
                        principalTable: "SupplierPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalConversations_SupplierQuotes_SupplierQuoteId",
                        column: x => x.SupplierQuoteId,
                        principalTable: "SupplierQuotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsResponsibilityTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FromPartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToPartyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncotermRuleId = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlannedTransferAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualTransferAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ContractualBasis = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    EvidenceSupplyDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsResponsibilityTransfers", x => x.Id);
                    table.CheckConstraint("CK_GoodsResponsibilityTransfers_Context", "num_nonnulls(\"BuyerSalesOrderId\", \"SupplierPurchaseOrderId\", \"ShipmentId\", \"ShipmentLegId\",\n    \"InventoryLotId\") >= 1");
                    table.CheckConstraint("CK_GoodsResponsibilityTransfers_DifferentParties", "\"FromPartyId\" <> \"ToPartyId\"");
                    table.CheckConstraint("CK_GoodsResponsibilityTransfers_ShipmentLeg", "\"ShipmentLegId\" IS NULL\nOR \"ShipmentId\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_BuyerSalesOrders_BuyerSalesOrd~",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_GeographicLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "GeographicLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_IncotermRules_IncotermRuleId",
                        column: x => x.IncotermRuleId,
                        principalTable: "IncotermRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_InventoryLots_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "InventoryLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_Parties_FromPartyId",
                        column: x => x.FromPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_Parties_ToPartyId",
                        column: x => x.ToPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_ShipmentLegs_ShipmentLegId_Shi~",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_SupplierPurchaseOrders_Supplie~",
                        column: x => x.SupplierPurchaseOrderId,
                        principalTable: "SupplierPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsResponsibilityTransfers_SupplyDocuments_EvidenceSupply~",
                        column: x => x.EvidenceSupplyDocumentId,
                        principalTable: "SupplyDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerSalesOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalAcceptanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerProposalAcceptanceLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    FulfilledQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    BuyerLineReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequiredDeliveryFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequiredDeliveryToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerSalesOrderLines", x => x.Id);
                    table.UniqueConstraint("AK_BuyerSalesOrderLines_Id_BuyerSalesOrderId", x => new { x.Id, x.BuyerSalesOrderId });
                    table.CheckConstraint("CK_BuyerSalesOrderLines_Values", "\"OrderedQuantity\" > 0\nAND \"FulfilledQuantity\" >= 0\nAND \"FulfilledQuantity\" <= \"OrderedQuantity\"\nAND \"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderLines_BuyerProposalAcceptanceLines_BuyerProp~",
                        columns: x => new { x.BuyerProposalAcceptanceLineId, x.BuyerProposalAcceptanceId },
                        principalTable: "BuyerProposalAcceptanceLines",
                        principalColumns: new[] { "Id", "BuyerProposalAcceptanceId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderLines_BuyerSalesOrders_BuyerSalesOrderId_Buy~",
                        columns: x => new { x.BuyerSalesOrderId, x.BuyerProposalAcceptanceId },
                        principalTable: "BuyerSalesOrders",
                        principalColumns: new[] { "Id", "BuyerProposalAcceptanceId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderLines_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPurchaseOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierQuoteLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierProcessCapabilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplySolutionStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    FulfilledQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    RequiredAvailabilityFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RequiredAvailabilityToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SupplierLineReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPurchaseOrderLines", x => x.Id);
                    table.UniqueConstraint("AK_SupplierPurchaseOrderLines_Id_SupplierBusinessRelationshipId", x => new { x.Id, x.SupplierBusinessRelationshipId });
                    table.CheckConstraint("CK_SupplierPurchaseOrderLines_Target", "num_nonnulls(\"SupplierMaterialOfferingId\", \"SupplierProcessCapabilityId\",\n    \"SupplySolutionStepId\", \"InventoryLotId\") >= 1");
                    table.CheckConstraint("CK_SupplierPurchaseOrderLines_Values", "\"OrderedQuantity\" > 0\nAND \"FulfilledQuantity\" >= 0\nAND \"FulfilledQuantity\" <= \"OrderedQuantity\"\nAND \"UnitPrice\" >= 0");
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_InventoryLots_InventoryLotId_Sup~",
                        columns: x => new { x.InventoryLotId, x.SupplierBusinessRelationshipId },
                        principalTable: "InventoryLots",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_SupplierMaterialOfferings_Suppli~",
                        columns: x => new { x.SupplierMaterialOfferingId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierMaterialOfferings",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_SupplierProcessCapabilities_Supp~",
                        columns: x => new { x.SupplierProcessCapabilityId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierProcessCapabilities",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_SupplierPurchaseOrders_SupplierP~",
                        columns: x => new { x.SupplierPurchaseOrderId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierPurchaseOrders",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_SupplierQuoteLines_SupplierQuote~",
                        columns: x => new { x.SupplierQuoteLineId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierQuoteLines",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_SupplySolutionSteps_SupplySoluti~",
                        column: x => x.SupplySolutionStepId,
                        principalTable: "SupplySolutionSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPurchaseOrderLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplyAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierBusinessRelationshipId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierMaterialOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierOfferingAvailabilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierQuoteLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AllocationSequence = table.Column<int>(type: "integer", nullable: false),
                    AllocatedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    UnitPurchasePrice = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    PurchasePriceCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    EstimatedAdditionalCost = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    AvailabilityVerificationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    QualityVerificationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyAllocations", x => x.Id);
                    table.UniqueConstraint("AK_SupplyAllocations_Id_SupplySolutionLineId", x => new { x.Id, x.SupplySolutionLineId });
                    table.CheckConstraint("CK_SupplyAllocations_PriceCurrency", "(\"UnitPurchasePrice\" IS NULL AND \"PurchasePriceCurrencyId\" IS NULL)\nOR (\"UnitPurchasePrice\" >= 0 AND \"PurchasePriceCurrencyId\" IS NOT NULL)");
                    table.CheckConstraint("CK_SupplyAllocations_Quantity", "\"AllocatedQuantity\" > 0");
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_BusinessRelationships_SupplierBusinessRel~",
                        column: x => x.SupplierBusinessRelationshipId,
                        principalTable: "BusinessRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_Currencies_PurchasePriceCurrencyId",
                        column: x => x.PurchasePriceCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_InventoryLots_InventoryLotId_SupplierBusi~",
                        columns: x => new { x.InventoryLotId, x.SupplierBusinessRelationshipId },
                        principalTable: "InventoryLots",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_SupplierMaterialOfferings_SupplierMateria~",
                        columns: x => new { x.SupplierMaterialOfferingId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierMaterialOfferings",
                        principalColumns: new[] { "Id", "BusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_SupplierOfferingAvailabilities_SupplierOf~",
                        columns: x => new { x.SupplierOfferingAvailabilityId, x.SupplierMaterialOfferingId },
                        principalTable: "SupplierOfferingAvailabilities",
                        principalColumns: new[] { "Id", "SupplierMaterialOfferingId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_SupplierQuoteLines_SupplierQuoteLineId_Su~",
                        columns: x => new { x.SupplierQuoteLineId, x.SupplierBusinessRelationshipId },
                        principalTable: "SupplierQuoteLines",
                        principalColumns: new[] { "Id", "SupplierBusinessRelationshipId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_SupplySolutionLines_SupplySolutionLineId",
                        column: x => x.SupplySolutionLineId,
                        principalTable: "SupplySolutionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyAllocations_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAccountingReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommercialInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: false),
                    ExternalEntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalEntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExternalEntityNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ExternalUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ExternalVersionToken = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LastPushedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastPulledAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SyncStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAccountingReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAccountingReferences_CommercialInvoices_CommercialI~",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalAccountingReferences_ExternalSystems_ExternalSystem~",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceSyncAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommercialInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequestPayloadHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ResponsePayloadHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExternalEntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ErrorCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    RetryNumber = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSyncAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceSyncAttempts_CommercialInvoices_CommercialInvoiceId",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceSyncAttempts_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatusSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommercialInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    ObservedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExternalPaymentReference = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatusSnapshots", x => x.Id);
                    table.CheckConstraint("CK_PaymentStatusSnapshots_Amounts", "\"PaidAmount\" >= 0\nAND \"OutstandingAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_PaymentStatusSnapshots_CommercialInvoices_CommercialInvoice~",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentStatusSnapshots_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommercialInvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationTasks", x => x.Id);
                    table.CheckConstraint("CK_ReconciliationTasks_Context", "num_nonnulls(\"CommercialInvoiceId\", \"BuyerSalesOrderId\", \"SupplierPurchaseOrderId\",\n    \"ShipmentId\") >= 1");
                    table.ForeignKey(
                        name: "FK_ReconciliationTasks_BuyerSalesOrders_BuyerSalesOrderId",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReconciliationTasks_CommercialInvoices_CommercialInvoiceId",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReconciliationTasks_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReconciliationTasks_SupplierPurchaseOrders_SupplierPurchase~",
                        column: x => x.SupplierPurchaseOrderId,
                        principalTable: "SupplierPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    InReplyToMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Direction = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Channel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SenderPersonId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipientPersonId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisclosurePackageRevisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalMessageId = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Body = table.Column<string>(type: "character varying(32000)", maxLength: 32000, nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RecordedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalMessages_DisclosurePackageRevisions_DisclosurePacka~",
                        column: x => x.DisclosurePackageRevisionId,
                        principalTable: "DisclosurePackageRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalMessages_ExternalConversations_ExternalConversation~",
                        column: x => x.ExternalConversationId,
                        principalTable: "ExternalConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalMessages_ExternalMessages_InReplyToMessageId",
                        column: x => x.InReplyToMessageId,
                        principalTable: "ExternalMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalMessages_People_RecipientPersonId",
                        column: x => x.RecipientPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExternalMessages_People_SenderPersonId",
                        column: x => x.SenderPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeExecutionCheckpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TradeExecutionCheckpointTypeId = table.Column<int>(type: "integer", nullable: false),
                    ParentCheckpointId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerProposalAcceptanceId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplySolutionStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommercialInvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    GoodsResponsibilityTransferId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalSystemId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CompletionMode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    WaivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    WaivedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AutomationHandlerKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ExternalReference = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CompletionEvidenceJson = table.Column<string>(type: "jsonb", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeExecutionCheckpoints", x => x.Id);
                    table.CheckConstraint("CK_TradeExecutionCheckpoints_Context", "num_nonnulls(\"ParentCheckpointId\", \"BuyerProposalAcceptanceId\", \"BuyerSalesOrderId\",\n    \"SupplierPurchaseOrderId\", \"SupplySolutionStepId\", \"ShipmentId\", \"CommercialInvoiceId\",\n    \"GoodsResponsibilityTransferId\") >= 1");
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_BuyerProposalAcceptances_BuyerPro~",
                        column: x => x.BuyerProposalAcceptanceId,
                        principalTable: "BuyerProposalAcceptances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_BuyerSalesOrders_BuyerSalesOrderId",
                        column: x => x.BuyerSalesOrderId,
                        principalTable: "BuyerSalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_CommercialInvoices_CommercialInvo~",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_GoodsResponsibilityTransfers_Good~",
                        column: x => x.GoodsResponsibilityTransferId,
                        principalTable: "GoodsResponsibilityTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_SupplierPurchaseOrders_SupplierPu~",
                        column: x => x.SupplierPurchaseOrderId,
                        principalTable: "SupplierPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_SupplySolutionSteps_SupplySolutio~",
                        column: x => x.SupplySolutionStepId,
                        principalTable: "SupplySolutionSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_TradeExecutionCheckpointTypes_Tra~",
                        column: x => x.TradeExecutionCheckpointTypeId,
                        principalTable: "TradeExecutionCheckpointTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeExecutionCheckpoints_TradeExecutionCheckpoints_ParentC~",
                        column: x => x.ParentCheckpointId,
                        principalTable: "TradeExecutionCheckpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerSalesOrderDeliveryLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderDeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerSalesOrderDeliveryLines", x => x.Id);
                    table.CheckConstraint("CK_BuyerSalesOrderDeliveryLines_Values", "\"PlannedQuantity\" > 0\nAND \"DeliveredQuantity\" >= 0\nAND \"DeliveredQuantity\" <= \"PlannedQuantity\"");
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveryLines_BuyerSalesOrderDeliveries_Buye~",
                        columns: x => new { x.BuyerSalesOrderDeliveryId, x.BuyerSalesOrderId },
                        principalTable: "BuyerSalesOrderDeliveries",
                        principalColumns: new[] { "Id", "BuyerSalesOrderId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveryLines_BuyerSalesOrderLines_BuyerSale~",
                        columns: x => new { x.BuyerSalesOrderLineId, x.BuyerSalesOrderId },
                        principalTable: "BuyerSalesOrderLines",
                        principalColumns: new[] { "Id", "BuyerSalesOrderId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyerSalesOrderDeliveryLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommercialInvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommercialInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    ExternalLineId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialInvoiceLines", x => x.Id);
                    table.CheckConstraint("CK_CommercialInvoiceLines_Values", "\"Quantity\" >= 0\nAND \"UnitPrice\" >= 0\nAND \"NetAmount\" >= 0\nAND \"TaxAmount\" >= 0\nAND \"GrossAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceLines_BuyerSalesOrderLines_BuyerSalesOrder~",
                        column: x => x.BuyerSalesOrderLineId,
                        principalTable: "BuyerSalesOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceLines_CommercialInvoices_CommercialInvoice~",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceLines_SupplierPurchaseOrderLines_SupplierP~",
                        column: x => x.SupplierPurchaseOrderLineId,
                        principalTable: "SupplierPurchaseOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceLines_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesPurchaseAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerSalesOrderLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierPurchaseOrderLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    AllocatedPurchaseCost = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    PurchaseCostCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPurchaseAllocations", x => x.Id);
                    table.CheckConstraint("CK_SalesPurchaseAllocations_Quantity", "\"AllocatedQuantity\" > 0");
                    table.ForeignKey(
                        name: "FK_SalesPurchaseAllocations_BuyerSalesOrderLines_BuyerSalesOrd~",
                        column: x => x.BuyerSalesOrderLineId,
                        principalTable: "BuyerSalesOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPurchaseAllocations_Currencies_PurchaseCostCurrencyId",
                        column: x => x.PurchaseCostCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPurchaseAllocations_SupplierPurchaseOrderLines_Supplie~",
                        column: x => x.SupplierPurchaseOrderLineId,
                        principalTable: "SupplierPurchaseOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesPurchaseAllocations_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentOrderLineAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentLegId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyerSalesOrderLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierPurchaseOrderLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllocatedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentOrderLineAllocations", x => x.Id);
                    table.CheckConstraint("CK_ShipmentOrderLineAllocations_OrderSide", "num_nonnulls(\"BuyerSalesOrderLineId\", \"SupplierPurchaseOrderLineId\") = 1");
                    table.CheckConstraint("CK_ShipmentOrderLineAllocations_Quantities", "\"AllocatedQuantity\" > 0\nAND \"DeliveredQuantity\" >= 0\nAND \"DeliveredQuantity\" <= \"AllocatedQuantity\"");
                    table.ForeignKey(
                        name: "FK_ShipmentOrderLineAllocations_BuyerSalesOrderLines_BuyerSale~",
                        column: x => x.BuyerSalesOrderLineId,
                        principalTable: "BuyerSalesOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentOrderLineAllocations_ShipmentLegs_ShipmentLegId_Shi~",
                        columns: x => new { x.ShipmentLegId, x.ShipmentId },
                        principalTable: "ShipmentLegs",
                        principalColumns: new[] { "Id", "ShipmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentOrderLineAllocations_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentOrderLineAllocations_SupplierPurchaseOrderLines_Sup~",
                        column: x => x.SupplierPurchaseOrderLineId,
                        principalTable: "SupplierPurchaseOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentOrderLineAllocations_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CapacityEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplyAllocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Outcome = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RequestedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true),
                    AllocatedQuantity = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    QuantityUnitId = table.Column<int>(type: "integer", nullable: false),
                    AvailabilityFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AvailabilityToUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Explanation = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapacityEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapacityEvaluations_SupplyAllocations_SupplyAllocationId_Su~",
                        columns: x => new { x.SupplyAllocationId, x.SupplySolutionLineId },
                        principalTable: "SupplyAllocations",
                        principalColumns: new[] { "Id", "SupplySolutionLineId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapacityEvaluations_SupplySolutionLines_SupplySolutionLineId",
                        column: x => x.SupplySolutionLineId,
                        principalTable: "SupplySolutionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CapacityEvaluations_UnitsOfMeasure_QuantityUnitId",
                        column: x => x.QuantityUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequirementEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerRfqRequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplyAllocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplySolutionStepId = table.Column<Guid>(type: "uuid", nullable: true),
                    Outcome = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsHardFail = table.Column<bool>(type: "boolean", nullable: false),
                    VerificationState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ActualNumericValue = table.Column<decimal>(type: "numeric(24,10)", precision: 24, scale: 10, nullable: true),
                    ActualTextValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    Explanation = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    EvidenceSnapshotJson = table.Column<string>(type: "jsonb", nullable: true),
                    RuleTraceJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementEvaluations", x => x.Id);
                    table.UniqueConstraint("AK_RequirementEvaluations_Id_SupplySolutionCandidateId", x => new { x.Id, x.SupplySolutionCandidateId });
                    table.CheckConstraint("CK_RequirementEvaluations_AllocationLine", "\"SupplyAllocationId\" IS NULL\nOR \"SupplySolutionLineId\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_RequirementEvaluations_BuyerRfqRequirements_BuyerRfqRequire~",
                        column: x => x.BuyerRfqRequirementId,
                        principalTable: "BuyerRfqRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequirementEvaluations_SupplyAllocations_SupplyAllocationId~",
                        columns: x => new { x.SupplyAllocationId, x.SupplySolutionLineId },
                        principalTable: "SupplyAllocations",
                        principalColumns: new[] { "Id", "SupplySolutionLineId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequirementEvaluations_SupplySolutionCandidates_SupplySolut~",
                        column: x => x.SupplySolutionCandidateId,
                        principalTable: "SupplySolutionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequirementEvaluations_SupplySolutionLines_SupplySolutionLi~",
                        columns: x => new { x.SupplySolutionLineId, x.SupplySolutionCandidateId },
                        principalTable: "SupplySolutionLines",
                        principalColumns: new[] { "Id", "SupplySolutionCandidateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequirementEvaluations_SupplySolutionSteps_SupplySolutionSt~",
                        columns: x => new { x.SupplySolutionStepId, x.SupplySolutionCandidateId },
                        principalTable: "SupplySolutionSteps",
                        principalColumns: new[] { "Id", "SupplySolutionCandidateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequirementEvaluations_UnitsOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalMessageAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplyDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FileName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StorageProvider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StorageKey = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalMessageAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalMessageAttachments_ExternalMessages_ExternalMessage~",
                        column: x => x.ExternalMessageId,
                        principalTable: "ExternalMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalMessageAttachments_SupplyDocuments_SupplyDocumentId",
                        column: x => x.SupplyDocumentId,
                        principalTable: "SupplyDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScoreContributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplySolutionCandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementEvaluationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Weight = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: false),
                    NormalizedScore = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: false),
                    Points = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: false),
                    Explanation = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreateDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ActionDateUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreContributions_RequirementEvaluations_RequirementEvalua~",
                        columns: x => new { x.RequirementEvaluationId, x.SupplySolutionCandidateId },
                        principalTable: "RequirementEvaluations",
                        principalColumns: new[] { "Id", "SupplySolutionCandidateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoreContributions_SupplySolutionCandidates_SupplySolutionC~",
                        column: x => x.SupplySolutionCandidateId,
                        principalTable: "SupplySolutionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AddressKey",
                table: "Addresses",
                column: "AddressKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceCharacteristics_BiologicalSourceId_Characte~",
                table: "BiologicalSourceCharacteristics",
                columns: new[] { "BiologicalSourceId", "CharacteristicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceCharacteristics_CharacteristicId",
                table: "BiologicalSourceCharacteristics",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceConstituents_BiologicalSourceId_Constituent~",
                table: "BiologicalSourceConstituents",
                columns: new[] { "BiologicalSourceId", "ConstituentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceConstituents_ConstituentId",
                table: "BiologicalSourceConstituents",
                column: "ConstituentId");

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceNames_BiologicalSourceId_NameType_LanguageC~",
                table: "BiologicalSourceNames",
                columns: new[] { "BiologicalSourceId", "NameType", "LanguageCode", "NormalizedName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceReferences_BiologicalSourceId_ReferenceEntr~",
                table: "BiologicalSourceReferences",
                columns: new[] { "BiologicalSourceId", "ReferenceEntryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceReferences_BiologicalSourceId_SortOrder",
                table: "BiologicalSourceReferences",
                columns: new[] { "BiologicalSourceId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceReferences_ReferenceEntryId",
                table: "BiologicalSourceReferences",
                column: "ReferenceEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceRegions_BiologicalSourceId_GeographicLocati~",
                table: "BiologicalSourceRegions",
                columns: new[] { "BiologicalSourceId", "GeographicLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSourceRegions_GeographicLocationId",
                table: "BiologicalSourceRegions",
                column: "GeographicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSources_ParentBiologicalSourceId",
                table: "BiologicalSources",
                column: "ParentBiologicalSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSources_ParentBiologicalSourceId_DeclaredRank_Nor~",
                table: "BiologicalSources",
                columns: new[] { "ParentBiologicalSourceId", "DeclaredRank", "NormalizedName" });

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSources_Slug",
                table: "BiologicalSources",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiologicalSources_TaxonId",
                table: "BiologicalSources",
                column: "TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaimEvidence_BusinessR~",
                table: "BusinessRelationshipBiologicalSourceClaimEvidence",
                columns: new[] { "BusinessRelationshipBiologicalSourceClaimId", "SourceSystem", "SourceRecordKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaimRegions_BusinessR~1",
                table: "BusinessRelationshipBiologicalSourceClaimRegions",
                columns: new[] { "BusinessRelationshipBiologicalSourceClaimId", "GeographicLocationId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaimRegions_BusinessRe~",
                table: "BusinessRelationshipBiologicalSourceClaimRegions",
                column: "BusinessRelationshipBiologicalSourceClaimId",
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaimRegions_Geographic~",
                table: "BusinessRelationshipBiologicalSourceClaimRegions",
                column: "GeographicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaims_BiologicalSource~",
                table: "BusinessRelationshipBiologicalSourceClaims",
                column: "BiologicalSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaims_BusinessRelatio~1",
                table: "BusinessRelationshipBiologicalSourceClaims",
                columns: new[] { "BusinessRelationshipId", "RelationshipKind", "NormalizedDeclaredName" },
                unique: true,
                filter: "\"BiologicalSourceId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaims_BusinessRelation~",
                table: "BusinessRelationshipBiologicalSourceClaims",
                columns: new[] { "BusinessRelationshipId", "RelationshipKind", "BiologicalSourceId" },
                unique: true,
                filter: "\"BiologicalSourceId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipBiologicalSourceClaims_VerificationStat~",
                table: "BusinessRelationshipBiologicalSourceClaims",
                columns: new[] { "VerificationStatus", "LastObservedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipContacts_BusinessRelationshipId",
                table: "BusinessRelationshipContacts",
                column: "BusinessRelationshipId",
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipContacts_BusinessRelationshipId_Organiz~",
                table: "BusinessRelationshipContacts",
                columns: new[] { "BusinessRelationshipId", "OrganizationPersonId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipContacts_OrganizationPersonId",
                table: "BusinessRelationshipContacts",
                column: "OrganizationPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationships_BusinessRelationshipTypeId_BusinessRe~",
                table: "BusinessRelationships",
                columns: new[] { "BusinessRelationshipTypeId", "BusinessRelationshipStageId" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationships_CounterpartyId",
                table: "BusinessRelationships",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationships_NextActionUtc",
                table: "BusinessRelationships",
                column: "NextActionUtc");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationships_OwnerUserId",
                table: "BusinessRelationships",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationships_OwningOrganizationId_CounterpartyId_B~",
                table: "BusinessRelationships",
                columns: new[] { "OwningOrganizationId", "CounterpartyId", "BusinessRelationshipTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipStageHistory_BusinessRelationshipId_Cha~",
                table: "BusinessRelationshipStageHistory",
                columns: new[] { "BusinessRelationshipId", "ChangedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipStageHistory_FromStageId",
                table: "BusinessRelationshipStageHistory",
                column: "FromStageId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipStageHistory_ToStageId",
                table: "BusinessRelationshipStageHistory",
                column: "ToStageId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipStages_BusinessRelationshipTypeId_Code",
                table: "BusinessRelationshipStages",
                columns: new[] { "BusinessRelationshipTypeId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipStages_BusinessRelationshipTypeId_SortO~",
                table: "BusinessRelationshipStages",
                columns: new[] { "BusinessRelationshipTypeId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRelationshipTypes_Code",
                table: "BusinessRelationshipTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandEvidence_BuyerDemandId_ObservedAtUtc",
                table: "BuyerDemandEvidence",
                columns: new[] { "BuyerDemandId", "ObservedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandEvidence_CrmActivityId",
                table: "BuyerDemandEvidence",
                column: "CrmActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandEvidence_EvidenceHash",
                table: "BuyerDemandEvidence",
                column: "EvidenceHash");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandEvidence_ReferenceEntryId",
                table: "BuyerDemandEvidence",
                column: "ReferenceEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemands_BusinessRelationshipId_Status",
                table: "BuyerDemands",
                columns: new[] { "BusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemands_KnowledgeLevel_LastConfirmedAtUtc",
                table: "BuyerDemands",
                columns: new[] { "KnowledgeLevel", "LastConfirmedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_BiologicalSourceClaimId_BusinessRelations~",
                table: "BuyerDemandScopes",
                columns: new[] { "BiologicalSourceClaimId", "BusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_BuyerDemandId_BusinessRelationshipId",
                table: "BuyerDemandScopes",
                columns: new[] { "BuyerDemandId", "BusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_BuyerDemandId_SortOrder",
                table: "BuyerDemandScopes",
                columns: new[] { "BuyerDemandId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_IntendedUseCharacteristicId",
                table: "BuyerDemandScopes",
                column: "IntendedUseCharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_MaterialId",
                table: "BuyerDemandScopes",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_MaterialTypeId",
                table: "BuyerDemandScopes",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandScopes_TypicalQuantityUnitId",
                table: "BuyerDemandScopes",
                column: "TypicalQuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandStateHistory_BuyerDemandEvidenceId_BuyerDemandId",
                table: "BuyerDemandStateHistory",
                columns: new[] { "BuyerDemandEvidenceId", "BuyerDemandId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDemandStateHistory_BuyerDemandId_ChangedAtUtc",
                table: "BuyerDemandStateHistory",
                columns: new[] { "BuyerDemandId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptanceLines_BuyerProposalAcceptanceId_Buy~1",
                table: "BuyerProposalAcceptanceLines",
                columns: new[] { "BuyerProposalAcceptanceId", "BuyerProposalOptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptanceLines_BuyerProposalAcceptanceId_Buye~",
                table: "BuyerProposalAcceptanceLines",
                columns: new[] { "BuyerProposalAcceptanceId", "BuyerProposalLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptanceLines_BuyerProposalLineId_BuyerPropo~",
                table: "BuyerProposalAcceptanceLines",
                columns: new[] { "BuyerProposalLineId", "BuyerProposalOptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptanceLines_QuantityUnitId",
                table: "BuyerProposalAcceptanceLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptances_BuyerProposalOptionId_BuyerProposa~",
                table: "BuyerProposalAcceptances",
                columns: new[] { "BuyerProposalOptionId", "BuyerProposalRevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptances_BuyerProposalOptionId_Status",
                table: "BuyerProposalAcceptances",
                columns: new[] { "BuyerProposalOptionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalAcceptances_BuyerProposalRevisionId_BuyerPropo~",
                table: "BuyerProposalAcceptances",
                columns: new[] { "BuyerProposalRevisionId", "BuyerProposalOptionId", "AcceptedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_BuyerProposalAcceptances_OnePendingPerRevision",
                table: "BuyerProposalAcceptances",
                column: "BuyerProposalRevisionId",
                unique: true,
                filter: "\"Status\" = 'PendingInternalApproval'");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalLines_BuyerProposalOptionId_LineNumber",
                table: "BuyerProposalLines",
                columns: new[] { "BuyerProposalOptionId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalLines_BuyerRfqLineId",
                table: "BuyerProposalLines",
                column: "BuyerRfqLineId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalLines_MaterialId",
                table: "BuyerProposalLines",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalLines_ProductId",
                table: "BuyerProposalLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalLines_QuantityUnitId",
                table: "BuyerProposalLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalLines_SupplySolutionLineId",
                table: "BuyerProposalLines",
                column: "SupplySolutionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalOptions_BuyerProposalRevisionId_OptionNumber",
                table: "BuyerProposalOptions",
                columns: new[] { "BuyerProposalRevisionId", "OptionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalOptions_CurrencyId",
                table: "BuyerProposalOptions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalOptions_SupplySolutionCandidateId",
                table: "BuyerProposalOptions",
                column: "SupplySolutionCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisions_BuyerProposalId_BuyerRfqId",
                table: "BuyerProposalRevisions",
                columns: new[] { "BuyerProposalId", "BuyerRfqId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisions_BuyerProposalId_RevisionNumber",
                table: "BuyerProposalRevisions",
                columns: new[] { "BuyerProposalId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisions_BuyerRfqRevisionId_BuyerRfqId",
                table: "BuyerProposalRevisions",
                columns: new[] { "BuyerRfqRevisionId", "BuyerRfqId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisions_CurrencyId",
                table: "BuyerProposalRevisions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisions_DisclosurePackageRevisionId",
                table: "BuyerProposalRevisions",
                column: "DisclosurePackageRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisions_SnapshotHash",
                table: "BuyerProposalRevisions",
                column: "SnapshotHash");

            migrationBuilder.CreateIndex(
                name: "UX_BuyerProposalRevisions_OneIssued",
                table: "BuyerProposalRevisions",
                column: "BuyerProposalId",
                unique: true,
                filter: "\"Status\" = 'Issued'");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposalRevisionScheduledTaskLinks_Revision_Purpose",
                table: "BuyerProposalRevisionScheduledTaskLinks",
                columns: new[] { "BuyerProposalRevisionId", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "UX_BuyerProposalRevisionScheduledTaskLinks_ScheduledTask",
                table: "BuyerProposalRevisionScheduledTaskLinks",
                column: "ScheduledTaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposals_BuyerBusinessRelationshipId_Status",
                table: "BuyerProposals",
                columns: new[] { "BuyerBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposals_BuyerRfqId_BuyerBusinessRelationshipId",
                table: "BuyerProposals",
                columns: new[] { "BuyerRfqId", "BuyerBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerProposals_ProposalNumber",
                table: "BuyerProposals",
                column: "ProposalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveries_BuyerRfqRevisionId_DeliveryNumber",
                table: "BuyerRfqDeliveries",
                columns: new[] { "BuyerRfqRevisionId", "DeliveryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveries_DestinationAddressId",
                table: "BuyerRfqDeliveries",
                column: "DestinationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveries_DestinationLocationId",
                table: "BuyerRfqDeliveries",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveries_IncotermRuleId",
                table: "BuyerRfqDeliveries",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveryLineAllocations_BuyerRfqDeliveryId_BuyerRf~1",
                table: "BuyerRfqDeliveryLineAllocations",
                columns: new[] { "BuyerRfqDeliveryId", "BuyerRfqRevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveryLineAllocations_BuyerRfqDeliveryId_BuyerRfq~",
                table: "BuyerRfqDeliveryLineAllocations",
                columns: new[] { "BuyerRfqDeliveryId", "BuyerRfqLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveryLineAllocations_BuyerRfqLineId_BuyerRfqRevi~",
                table: "BuyerRfqDeliveryLineAllocations",
                columns: new[] { "BuyerRfqLineId", "BuyerRfqRevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqDeliveryLineAllocations_QuantityUnitId",
                table: "BuyerRfqDeliveryLineAllocations",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLineMaterialOptions_BiologicalSourceClaimId",
                table: "BuyerRfqLineMaterialOptions",
                column: "BiologicalSourceClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLineMaterialOptions_BiologicalSourceId",
                table: "BuyerRfqLineMaterialOptions",
                column: "BiologicalSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLineMaterialOptions_BuyerRfqLineId_OptionKind_SortO~",
                table: "BuyerRfqLineMaterialOptions",
                columns: new[] { "BuyerRfqLineId", "OptionKind", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLineMaterialOptions_MaterialId",
                table: "BuyerRfqLineMaterialOptions",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLineMaterialOptions_MaterialTypeId",
                table: "BuyerRfqLineMaterialOptions",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLines_BuyerDemandScopeId",
                table: "BuyerRfqLines",
                column: "BuyerDemandScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLines_BuyerRfqRevisionId_LineNumber",
                table: "BuyerRfqLines",
                columns: new[] { "BuyerRfqRevisionId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLines_IntendedUseCharacteristicId",
                table: "BuyerRfqLines",
                column: "IntendedUseCharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLines_MaterialId",
                table: "BuyerRfqLines",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLines_MaterialTypeId",
                table: "BuyerRfqLines",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqLines_QuantityUnitId",
                table: "BuyerRfqLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_BiologicalSourceId",
                table: "BuyerRfqRequirements",
                column: "BiologicalSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_BuyerRfqCharacteristicRequirement_Unit~",
                table: "BuyerRfqRequirements",
                column: "BuyerRfqCharacteristicRequirement_UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_BuyerRfqCommercialRequirement_UnitOfMe~",
                table: "BuyerRfqRequirements",
                column: "BuyerRfqCommercialRequirement_UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_BuyerRfqConstituentRequirement_UnitOfM~",
                table: "BuyerRfqRequirements",
                column: "BuyerRfqConstituentRequirement_UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_BuyerRfqLineId_BuyerRfqRevisionId",
                table: "BuyerRfqRequirements",
                columns: new[] { "BuyerRfqLineId", "BuyerRfqRevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_BuyerRfqRevisionId_BuyerRfqLineId_Sort~",
                table: "BuyerRfqRequirements",
                columns: new[] { "BuyerRfqRevisionId", "BuyerRfqLineId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_CertificationSchemeId",
                table: "BuyerRfqRequirements",
                column: "CertificationSchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_CertificationSchemeJurisdictionId_Cert~",
                table: "BuyerRfqRequirements",
                columns: new[] { "CertificationSchemeJurisdictionId", "CertificationSchemeId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_CharacteristicId",
                table: "BuyerRfqRequirements",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_ConstituentId",
                table: "BuyerRfqRequirements",
                column: "ConstituentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_CountryId",
                table: "BuyerRfqRequirements",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_CurrencyId",
                table: "BuyerRfqRequirements",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_DocumentTypeId",
                table: "BuyerRfqRequirements",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_GeographicLocationId",
                table: "BuyerRfqRequirements",
                column: "GeographicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_IncotermRuleId",
                table: "BuyerRfqRequirements",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_Level_IsActive",
                table: "BuyerRfqRequirements",
                columns: new[] { "Level", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_MaterialId",
                table: "BuyerRfqRequirements",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_MaterialTypeId",
                table: "BuyerRfqRequirements",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_ProcessTypeId",
                table: "BuyerRfqRequirements",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRequirements_UnitOfMeasureId",
                table: "BuyerRfqRequirements",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRevisions_BuyerRfqId_RevisionNumber",
                table: "BuyerRfqRevisions",
                columns: new[] { "BuyerRfqId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqRevisions_BuyerRfqId_Status",
                table: "BuyerRfqRevisions",
                columns: new[] { "BuyerRfqId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_BuyerRfqRevisions_OneReleasedPerRfq",
                table: "BuyerRfqRevisions",
                column: "BuyerRfqId",
                unique: true,
                filter: "\"Status\" = 'ReleasedForMatching'");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqs_BuyerBusinessRelationshipId_Status",
                table: "BuyerRfqs",
                columns: new[] { "BuyerBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqs_BuyerDemandId_BuyerBusinessRelationshipId",
                table: "BuyerRfqs",
                columns: new[] { "BuyerDemandId", "BuyerBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqs_OwnerUserId_Status",
                table: "BuyerRfqs",
                columns: new[] { "OwnerUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqs_RfqNumber",
                table: "BuyerRfqs",
                column: "RfqNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerRfqScheduledTaskLinks_Rfq_Purpose",
                table: "BuyerRfqScheduledTaskLinks",
                columns: new[] { "BuyerRfqId", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "UX_BuyerRfqScheduledTaskLinks_ScheduledTask",
                table: "BuyerRfqScheduledTaskLinks",
                column: "ScheduledTaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveries_BuyerSalesOrderId_DeliveryNumber",
                table: "BuyerSalesOrderDeliveries",
                columns: new[] { "BuyerSalesOrderId", "DeliveryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveries_DestinationAddressId",
                table: "BuyerSalesOrderDeliveries",
                column: "DestinationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveries_DestinationLocationId",
                table: "BuyerSalesOrderDeliveries",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveries_IncotermRuleId",
                table: "BuyerSalesOrderDeliveries",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveryLines_BuyerSalesOrderDeliveryId_Buy~1",
                table: "BuyerSalesOrderDeliveryLines",
                columns: new[] { "BuyerSalesOrderDeliveryId", "BuyerSalesOrderLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveryLines_BuyerSalesOrderDeliveryId_Buye~",
                table: "BuyerSalesOrderDeliveryLines",
                columns: new[] { "BuyerSalesOrderDeliveryId", "BuyerSalesOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveryLines_BuyerSalesOrderLineId_BuyerSal~",
                table: "BuyerSalesOrderDeliveryLines",
                columns: new[] { "BuyerSalesOrderLineId", "BuyerSalesOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderDeliveryLines_QuantityUnitId",
                table: "BuyerSalesOrderDeliveryLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderLines_BuyerProposalAcceptanceLineId_BuyerPro~",
                table: "BuyerSalesOrderLines",
                columns: new[] { "BuyerProposalAcceptanceLineId", "BuyerProposalAcceptanceId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderLines_BuyerSalesOrderId_BuyerProposalAccepta~",
                table: "BuyerSalesOrderLines",
                columns: new[] { "BuyerSalesOrderId", "BuyerProposalAcceptanceId" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderLines_BuyerSalesOrderId_LineNumber",
                table: "BuyerSalesOrderLines",
                columns: new[] { "BuyerSalesOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderLines_MaterialId",
                table: "BuyerSalesOrderLines",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderLines_ProductId",
                table: "BuyerSalesOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderLines_QuantityUnitId",
                table: "BuyerSalesOrderLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrders_BuyerBusinessRelationshipId_Status",
                table: "BuyerSalesOrders",
                columns: new[] { "BuyerBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrders_BuyerProposalAcceptanceId",
                table: "BuyerSalesOrders",
                column: "BuyerProposalAcceptanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrders_CurrencyId",
                table: "BuyerSalesOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrders_IncotermRuleId",
                table: "BuyerSalesOrders",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrders_OrderNumber",
                table: "BuyerSalesOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSalesOrderStatusHistory_BuyerSalesOrderId_ChangedAtUtc",
                table: "BuyerSalesOrderStatusHistory",
                columns: new[] { "BuyerSalesOrderId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CapacityEvaluations_QuantityUnitId",
                table: "CapacityEvaluations",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_CapacityEvaluations_SupplyAllocationId_SupplySolutionLineId",
                table: "CapacityEvaluations",
                columns: new[] { "SupplyAllocationId", "SupplySolutionLineId" });

            migrationBuilder.CreateIndex(
                name: "IX_CapacityEvaluations_SupplySolutionLineId_SupplyAllocationId",
                table: "CapacityEvaluations",
                columns: new[] { "SupplySolutionLineId", "SupplyAllocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarrierBookings_CarrierBusinessRelationshipId",
                table: "CarrierBookings",
                column: "CarrierBusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierBookings_CostCurrencyId",
                table: "CarrierBookings",
                column: "CostCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierBookings_ExternalSystemId",
                table: "CarrierBookings",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierBookings_ShipmentId_CarrierBusinessRelationshipId_St~",
                table: "CarrierBookings",
                columns: new[] { "ShipmentId", "CarrierBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CarrierBookings_ShipmentLegId_ShipmentId",
                table: "CarrierBookings",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CertificationSchemeJurisdictions_CertificationSchemeId_Code",
                table: "CertificationSchemeJurisdictions",
                columns: new[] { "CertificationSchemeId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificationSchemeJurisdictions_CountryId",
                table: "CertificationSchemeJurisdictions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationSchemes_Code",
                table: "CertificationSchemes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificationSchemes_Kind_IsActive",
                table: "CertificationSchemes",
                columns: new[] { "Kind", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_CharacteristicTypeId_NormalizedName",
                table: "Characteristics",
                columns: new[] { "CharacteristicTypeId", "NormalizedName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicTypes_Code",
                table: "CharacteristicTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoiceLines_BuyerSalesOrderLineId",
                table: "CommercialInvoiceLines",
                column: "BuyerSalesOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoiceLines_CommercialInvoiceId_LineNumber",
                table: "CommercialInvoiceLines",
                columns: new[] { "CommercialInvoiceId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoiceLines_QuantityUnitId",
                table: "CommercialInvoiceLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoiceLines_SupplierPurchaseOrderLineId",
                table: "CommercialInvoiceLines",
                column: "SupplierPurchaseOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_BuyerSalesOrderId",
                table: "CommercialInvoices",
                column: "BuyerSalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_CounterpartyBusinessRelationshipId_Statu~",
                table: "CommercialInvoices",
                columns: new[] { "CounterpartyBusinessRelationshipId", "Status", "DueDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_CurrencyId",
                table: "CommercialInvoices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_Direction_InvoiceNumber",
                table: "CommercialInvoices",
                columns: new[] { "Direction", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_ExternalSystemId",
                table: "CommercialInvoices",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_SupplierPurchaseOrderId",
                table: "CommercialInvoices",
                column: "SupplierPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoices_SyncStatus_ExternalSystemId",
                table: "CommercialInvoices",
                columns: new[] { "SyncStatus", "ExternalSystemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Constituents_ConstituentTypeId",
                table: "Constituents",
                column: "ConstituentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Constituents_NormalizedName",
                table: "Constituents",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConstituentTypes_Code",
                table: "ConstituentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactFunctionTypes_Code",
                table: "ContactFunctionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ContinentId",
                table: "Countries",
                column: "ContinentId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CurrencyId",
                table: "Countries",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryWater_WaterId",
                table: "CountryWater",
                column: "WaterId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivities_BusinessRelationshipId",
                table: "CrmActivities",
                column: "BusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivities_ExternalSystem_ExternalRecordId",
                table: "CrmActivities",
                columns: new[] { "ExternalSystem", "ExternalRecordId" },
                unique: true,
                filter: "\"ExternalSystem\" IS NOT NULL AND \"ExternalRecordId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivities_OwnerUserId_Status_DueUtc",
                table: "CrmActivities",
                columns: new[] { "OwnerUserId", "Status", "DueUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivities_RegardingPartyId_OccurredUtc",
                table: "CrmActivities",
                columns: new[] { "RegardingPartyId", "OccurredUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivityParticipants_CrmActivityId_PartyId_Role",
                table: "CrmActivityParticipants",
                columns: new[] { "CrmActivityId", "PartyId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivityParticipants_PartyId",
                table: "CrmActivityParticipants",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmActivityScheduledTaskLinks_Activity_Purpose",
                table: "CrmActivityScheduledTaskLinks",
                columns: new[] { "CrmActivityId", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "UX_CrmActivityScheduledTaskLinks_ScheduledTask",
                table: "CrmActivityScheduledTaskLinks",
                column: "ScheduledTaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRateChanges_CurrencyId_RateDate",
                table: "CurrencyExchangeRateChanges",
                columns: new[] { "CurrencyId", "RateDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomsReferences_CountryId",
                table: "CustomsReferences",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsReferences_ShipmentDocumentId",
                table: "CustomsReferences",
                column: "ShipmentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomsReferences_ShipmentId_Kind_ReferenceNumber",
                table: "CustomsReferences",
                columns: new[] { "ShipmentId", "Kind", "ReferenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisclosurePackageItems_DisclosurePackageRevisionId_FieldKey",
                table: "DisclosurePackageItems",
                columns: new[] { "DisclosurePackageRevisionId", "FieldKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisclosurePackageItems_SourceEntityType_SourceEntityId",
                table: "DisclosurePackageItems",
                columns: new[] { "SourceEntityType", "SourceEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_DisclosurePackageRevisions_ContentHash",
                table: "DisclosurePackageRevisions",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_DisclosurePackageRevisions_DisclosurePackageId_RevisionNumb~",
                table: "DisclosurePackageRevisions",
                columns: new[] { "DisclosurePackageId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisclosurePackages_AudienceBusinessRelationshipId_Status",
                table: "DisclosurePackages",
                columns: new[] { "AudienceBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DisclosurePackages_SourceEntityType_SourceEntityId",
                table: "DisclosurePackages",
                columns: new[] { "SourceEntityType", "SourceEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Category_IsActive",
                table: "DocumentTypes",
                columns: new[] { "Category", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Code",
                table: "DocumentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxMessages_FromUserId",
                table: "EmailOutboxMessages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxMessages_PartyId_Status",
                table: "EmailOutboxMessages",
                columns: new[] { "PartyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxMessages_ScheduledTaskId",
                table: "EmailOutboxMessages",
                column: "ScheduledTaskId",
                unique: true,
                filter: "\"ScheduledTaskId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxRecipients_OutboxMessageId_Type",
                table: "EmailOutboxRecipients",
                columns: new[] { "OutboxMessageId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxRecipients_OutboxMessageId_Type_UserId",
                table: "EmailOutboxRecipients",
                columns: new[] { "OutboxMessageId", "Type", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutboxRecipients_UserId",
                table: "EmailOutboxRecipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_Scope_Key",
                table: "EmailTemplate",
                columns: new[] { "Scope", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_Scope_Key_OrganisationId_UserId",
                table: "EmailTemplate",
                columns: new[] { "Scope", "Key", "OrganisationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateVariant_TemplateId_Culture",
                table: "EmailTemplateVariant",
                columns: new[] { "TemplateId", "Culture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateVariant_TemplateId_IsDefault",
                table: "EmailTemplateVariant",
                columns: new[] { "TemplateId", "IsDefault" },
                unique: true,
                filter: "\"IsDefault\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAccountingReferences_CommercialInvoiceId",
                table: "ExternalAccountingReferences",
                column: "CommercialInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAccountingReferences_ExternalSystemId_ExternalEntit~",
                table: "ExternalAccountingReferences",
                columns: new[] { "ExternalSystemId", "ExternalEntityType", "ExternalEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalConversations_BuyerProposalId",
                table: "ExternalConversations",
                column: "BuyerProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalConversations_BuyerRfqId",
                table: "ExternalConversations",
                column: "BuyerRfqId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalConversations_BuyerSalesOrderId",
                table: "ExternalConversations",
                column: "BuyerSalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalConversations_ExternalBusinessRelationshipId_Status~",
                table: "ExternalConversations",
                columns: new[] { "ExternalBusinessRelationshipId", "Status", "LastMessageAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalConversations_SupplierPurchaseOrderId",
                table: "ExternalConversations",
                column: "SupplierPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalConversations_SupplierQuoteId",
                table: "ExternalConversations",
                column: "SupplierQuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessageAttachments_ContentHash",
                table: "ExternalMessageAttachments",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessageAttachments_ExternalMessageId",
                table: "ExternalMessageAttachments",
                column: "ExternalMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessageAttachments_SupplyDocumentId",
                table: "ExternalMessageAttachments",
                column: "SupplyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessages_DisclosurePackageRevisionId",
                table: "ExternalMessages",
                column: "DisclosurePackageRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessages_ExternalConversationId_OccurredAtUtc",
                table: "ExternalMessages",
                columns: new[] { "ExternalConversationId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessages_ExternalMessageId",
                table: "ExternalMessages",
                column: "ExternalMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessages_InReplyToMessageId",
                table: "ExternalMessages",
                column: "InReplyToMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessages_RecipientPersonId",
                table: "ExternalMessages",
                column: "RecipientPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMessages_SenderPersonId",
                table: "ExternalMessages",
                column: "SenderPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalSystems_Code",
                table: "ExternalSystems",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalSystems_Kind_IsActive",
                table: "ExternalSystems",
                columns: new[] { "Kind", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_GeographicLocations_CountryId_WaterId",
                table: "GeographicLocations",
                columns: new[] { "CountryId", "WaterId" });

            migrationBuilder.CreateIndex(
                name: "IX_GeographicLocations_LocationKey",
                table: "GeographicLocations",
                column: "LocationKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeographicLocations_WaterId",
                table: "GeographicLocations",
                column: "WaterId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_BuyerSalesOrderId",
                table: "GoodsResponsibilityTransfers",
                column: "BuyerSalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_EvidenceSupplyDocumentId",
                table: "GoodsResponsibilityTransfers",
                column: "EvidenceSupplyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_FromPartyId",
                table: "GoodsResponsibilityTransfers",
                column: "FromPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_IncotermRuleId",
                table: "GoodsResponsibilityTransfers",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_InventoryLotId",
                table: "GoodsResponsibilityTransfers",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_Kind_Status_PlannedTransferAtU~",
                table: "GoodsResponsibilityTransfers",
                columns: new[] { "Kind", "Status", "PlannedTransferAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_LocationId",
                table: "GoodsResponsibilityTransfers",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_ShipmentId",
                table: "GoodsResponsibilityTransfers",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_ShipmentLegId_ShipmentId",
                table: "GoodsResponsibilityTransfers",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_SupplierPurchaseOrderId",
                table: "GoodsResponsibilityTransfers",
                column: "SupplierPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsResponsibilityTransfers_ToPartyId",
                table: "GoodsResponsibilityTransfers",
                column: "ToPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncotermRules_Code_Version",
                table: "IncotermRules",
                columns: new[] { "Code", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncotermRules_IsActive",
                table: "IncotermRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotLinks_ChildInventoryLotId",
                table: "InventoryLotLinks",
                column: "ChildInventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotLinks_MaterialTransformationId",
                table: "InventoryLotLinks",
                column: "MaterialTransformationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotLinks_ParentInventoryLotId_ChildInventoryLotId_~",
                table: "InventoryLotLinks",
                columns: new[] { "ParentInventoryLotId", "ChildInventoryLotId", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotLinks_QuantityUnitId",
                table: "InventoryLotLinks",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotOrigins_GeographicLocationId",
                table: "InventoryLotOrigins",
                column: "GeographicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotOrigins_InventoryLotId_GeographicLocationId_Role",
                table: "InventoryLotOrigins",
                columns: new[] { "InventoryLotId", "GeographicLocationId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLotOrigins_InventoryLotId_Role",
                table: "InventoryLotOrigins",
                columns: new[] { "InventoryLotId", "Role" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_CurrentLocationId",
                table: "InventoryLots",
                column: "CurrentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_LotNumber",
                table: "InventoryLots",
                column: "LotNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_MaterialId",
                table: "InventoryLots",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_QuantityUnitId",
                table: "InventoryLots",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_Status_BestBeforeUtc",
                table: "InventoryLots",
                columns: new[] { "Status", "BestBeforeUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_SupplierBusinessRelationshipId_SupplierLotNum~",
                table: "InventoryLots",
                columns: new[] { "SupplierBusinessRelationshipId", "SupplierLotNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLots_SupplierMaterialOfferingId_SupplierBusinessRe~",
                table: "InventoryLots",
                columns: new[] { "SupplierMaterialOfferingId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceSyncAttempts_CommercialInvoiceId_StartedAtUtc",
                table: "InvoiceSyncAttempts",
                columns: new[] { "CommercialInvoiceId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceSyncAttempts_ExternalSystemId",
                table: "InvoiceSyncAttempts",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceSyncAttempts_Status_StartedAtUtc",
                table: "InvoiceSyncAttempts",
                columns: new[] { "Status", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingModerationDecisions_SupplySolutionCandidateId_Decid~",
                table: "MatchingModerationDecisions",
                columns: new[] { "SupplySolutionCandidateId", "DecidedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuleSets_Code",
                table: "MatchingRuleSets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuleSets_Status",
                table: "MatchingRuleSets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuleSetVersions_ConfigurationHash",
                table: "MatchingRuleSetVersions",
                column: "ConfigurationHash");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuleSetVersions_MatchingRuleSetId_VersionNumber",
                table: "MatchingRuleSetVersions",
                columns: new[] { "MatchingRuleSetId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_MatchingRuleSetVersions_OneUnpublished",
                table: "MatchingRuleSetVersions",
                column: "MatchingRuleSetId",
                unique: true,
                filter: "\"PublishedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRunLines_BuyerRfqLineId_BuyerRfqRevisionId",
                table: "MatchingRunLines",
                columns: new[] { "BuyerRfqLineId", "BuyerRfqRevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRunLines_MatchingRunId_BuyerRfqLineId",
                table: "MatchingRunLines",
                columns: new[] { "MatchingRunId", "BuyerRfqLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRunLines_MatchingRunId_BuyerRfqRevisionId",
                table: "MatchingRunLines",
                columns: new[] { "MatchingRunId", "BuyerRfqRevisionId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRunLines_QuantityUnitId",
                table: "MatchingRunLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuns_BuyerRfqRevisionId_QueuedAtUtc",
                table: "MatchingRuns",
                columns: new[] { "BuyerRfqRevisionId", "QueuedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuns_MatchingRuleSetVersionId",
                table: "MatchingRuns",
                column: "MatchingRuleSetVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuns_Status_QueuedAtUtc",
                table: "MatchingRuns",
                columns: new[] { "Status", "QueuedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingRuns_Status_StartedAtUtc",
                table: "MatchingRuns",
                columns: new[] { "Status", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialBiologicalSources_BiologicalSourceId",
                table: "MaterialBiologicalSources",
                column: "BiologicalSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialBiologicalSources_MaterialId_BiologicalSourceId",
                table: "MaterialBiologicalSources",
                columns: new[] { "MaterialId", "BiologicalSourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCharacteristics_CharacteristicId",
                table: "MaterialCharacteristics",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialCharacteristics_MaterialId_CharacteristicId",
                table: "MaterialCharacteristics",
                columns: new[] { "MaterialId", "CharacteristicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialConstituents_ConstituentId",
                table: "MaterialConstituents",
                column: "ConstituentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialConstituents_MaterialId_ConstituentId",
                table: "MaterialConstituents",
                columns: new[] { "MaterialId", "ConstituentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrigins_GeographicLocationId",
                table: "MaterialOrigins",
                column: "GeographicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialOrigins_MaterialId_GeographicLocationId_Role",
                table: "MaterialOrigins",
                columns: new[] { "MaterialId", "GeographicLocationId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReferences_MaterialId_ReferenceEntryId",
                table: "MaterialReferences",
                columns: new[] { "MaterialId", "ReferenceEntryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReferences_MaterialId_SortOrder",
                table: "MaterialReferences",
                columns: new[] { "MaterialId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReferences_ReferenceEntryId",
                table: "MaterialReferences",
                column: "ReferenceEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Slug",
                table: "Materials",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformationInputs_MaterialId",
                table: "MaterialTransformationInputs",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformationInputs_MaterialTransformationId_Mater~",
                table: "MaterialTransformationInputs",
                columns: new[] { "MaterialTransformationId", "MaterialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformationInputs_UnitOfMeasureId",
                table: "MaterialTransformationInputs",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformationOutputs_MaterialId",
                table: "MaterialTransformationOutputs",
                column: "MaterialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformationOutputs_MaterialTransformationId_Mate~",
                table: "MaterialTransformationOutputs",
                columns: new[] { "MaterialTransformationId", "MaterialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformationOutputs_UnitOfMeasureId",
                table: "MaterialTransformationOutputs",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransformations_ProcessTypeId",
                table: "MaterialTransformations",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypes_Code",
                table: "MaterialTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypes_ParentMaterialTypeId",
                table: "MaterialTypes",
                column: "ParentMaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_IdentifierKey",
                table: "OrganizationIdentifiers",
                column: "IdentifierKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_IssuingCountryId",
                table: "OrganizationIdentifiers",
                column: "IssuingCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_OrganizationId",
                table: "OrganizationIdentifiers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationIdentifiers_OrganizationId_Scheme",
                table: "OrganizationIdentifiers",
                columns: new[] { "OrganizationId", "Scheme" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPeople_OrganizationId",
                table: "OrganizationPeople",
                column: "OrganizationId",
                unique: true,
                filter: "\"IsPrimaryContact\" = TRUE AND \"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPeople_OrganizationId_PersonId",
                table: "OrganizationPeople",
                columns: new[] { "OrganizationId", "PersonId" },
                unique: true,
                filter: "\"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPeople_PersonId",
                table: "OrganizationPeople",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPersonFunctions_ContactFunctionTypeId",
                table: "OrganizationPersonFunctions",
                column: "ContactFunctionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPersonFunctions_OrganizationPersonId_ContactFun~",
                table: "OrganizationPersonFunctions",
                columns: new[] { "OrganizationPersonId", "ContactFunctionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_NormalizedLegalName",
                table: "Organizations",
                column: "NormalizedLegalName");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentOrganizationId",
                table: "Organizations",
                column: "ParentOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_RegistrationCountryId",
                table: "Organizations",
                column: "RegistrationCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_IsActive",
                table: "Parties",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_NormalizedName",
                table: "Parties",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_PartyAddresses_AddressId",
                table: "PartyAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyAddresses_PartyId_AddressId_UsageType",
                table: "PartyAddresses",
                columns: new[] { "PartyId", "AddressId", "UsageType" },
                unique: true,
                filter: "\"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartyAddresses_PartyId_UsageType",
                table: "PartyAddresses",
                columns: new[] { "PartyId", "UsageType" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE AND \"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactPoints_OrganizationPersonId_Kind",
                table: "PartyContactPoints",
                columns: new[] { "OrganizationPersonId", "Kind" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE AND \"OrganizationPersonId\" IS NOT NULL AND \"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactPoints_OrganizationPersonId_Kind_NormalizedValue",
                table: "PartyContactPoints",
                columns: new[] { "OrganizationPersonId", "Kind", "NormalizedValue" },
                unique: true,
                filter: "\"OrganizationPersonId\" IS NOT NULL AND \"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactPoints_OrganizationPersonId_PartyId",
                table: "PartyContactPoints",
                columns: new[] { "OrganizationPersonId", "PartyId" });

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactPoints_PartyId_Kind",
                table: "PartyContactPoints",
                columns: new[] { "PartyId", "Kind" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE AND \"OrganizationPersonId\" IS NULL AND \"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartyContactPoints_PartyId_Kind_NormalizedValue",
                table: "PartyContactPoints",
                columns: new[] { "PartyId", "Kind", "NormalizedValue" },
                unique: true,
                filter: "\"OrganizationPersonId\" IS NULL AND \"ValidToUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartyExternalReferences_PartyId",
                table: "PartyExternalReferences",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyExternalReferences_SourceSystem_SourceRecordKey",
                table: "PartyExternalReferences",
                columns: new[] { "SourceSystem", "SourceRecordKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStatusSnapshots_CommercialInvoiceId_ObservedAtUtc",
                table: "PaymentStatusSnapshots",
                columns: new[] { "CommercialInvoiceId", "ObservedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStatusSnapshots_ExternalSystemId",
                table: "PaymentStatusSnapshots",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_People_FamilyName_GivenName",
                table: "People",
                columns: new[] { "FamilyName", "GivenName" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonUserAccounts_ApplicationUserId",
                table: "PersonUserAccounts",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonUserAccounts_AuthEmailContactPointId",
                table: "PersonUserAccounts",
                column: "AuthEmailContactPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonUserAccounts_IsExternalUser_Status",
                table: "PersonUserAccounts",
                columns: new[] { "IsExternalUser", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonUserAccounts_PersonId",
                table: "PersonUserAccounts",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTypes_Code",
                table: "ProcessTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCharacteristics_CharacteristicId",
                table: "ProductCharacteristics",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCharacteristics_ProductId_CharacteristicId",
                table: "ProductCharacteristics",
                columns: new[] { "ProductId", "CharacteristicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductIdentifiers_IdentifierKey",
                table: "ProductIdentifiers",
                column: "IdentifierKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductIdentifiers_ProductId",
                table: "ProductIdentifiers",
                column: "ProductId",
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferences_ProductId_ReferenceEntryId",
                table: "ProductReferences",
                columns: new[] { "ProductId", "ReferenceEntryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferences_ProductId_SortOrder",
                table: "ProductReferences",
                columns: new[] { "ProductId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReferences_ReferenceEntryId",
                table: "ProductReferences",
                column: "ReferenceEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MaterialId",
                table: "Products",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_NetContentUnitOfMeasureId",
                table: "Products",
                column: "NetContentUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_NormalizedSku",
                table: "Products",
                column: "NormalizedSku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SalesUnitOfMeasureId",
                table: "Products",
                column: "SalesUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Status_AvailableFromUtc",
                table: "Products",
                columns: new[] { "Status", "AvailableFromUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderEmailTemplateMapping_Provider_ProviderTemplateId",
                table: "ProviderEmailTemplateMapping",
                columns: new[] { "Provider", "ProviderTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderEmailTemplateMapping_TemplateVariantId_Provider",
                table: "ProviderEmailTemplateMapping",
                columns: new[] { "TemplateVariantId", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationTasks_BuyerSalesOrderId",
                table: "ReconciliationTasks",
                column: "BuyerSalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationTasks_CommercialInvoiceId",
                table: "ReconciliationTasks",
                column: "CommercialInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationTasks_ShipmentId",
                table: "ReconciliationTasks",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationTasks_Status_AssignedUserId_DueAtUtc",
                table: "ReconciliationTasks",
                columns: new[] { "Status", "AssignedUserId", "DueAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationTasks_SupplierPurchaseOrderId",
                table: "ReconciliationTasks",
                column: "SupplierPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceEntries_ReferenceKey",
                table: "ReferenceEntries",
                column: "ReferenceKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_BuyerRfqRequirementId",
                table: "RequirementEvaluations",
                column: "BuyerRfqRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_Outcome_IsHardFail",
                table: "RequirementEvaluations",
                columns: new[] { "Outcome", "IsHardFail" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_SupplyAllocationId_SupplySolutionLin~",
                table: "RequirementEvaluations",
                columns: new[] { "SupplyAllocationId", "SupplySolutionLineId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_SupplySolutionCandidateId_BuyerRfqRe~",
                table: "RequirementEvaluations",
                columns: new[] { "SupplySolutionCandidateId", "BuyerRfqRequirementId", "SupplyAllocationId", "SupplySolutionStepId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_SupplySolutionLineId_SupplySolutionC~",
                table: "RequirementEvaluations",
                columns: new[] { "SupplySolutionLineId", "SupplySolutionCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_SupplySolutionStepId_SupplySolutionC~",
                table: "RequirementEvaluations",
                columns: new[] { "SupplySolutionStepId", "SupplySolutionCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequirementEvaluations_UnitOfMeasureId",
                table: "RequirementEvaluations",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_SortOrder",
                table: "Role",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId_ClaimType_ClaimValue",
                table: "RoleClaims",
                columns: new[] { "RoleId", "ClaimType", "ClaimValue" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPurchaseAllocations_BuyerSalesOrderLineId_SupplierPurc~",
                table: "SalesPurchaseAllocations",
                columns: new[] { "BuyerSalesOrderLineId", "SupplierPurchaseOrderLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesPurchaseAllocations_PurchaseCostCurrencyId",
                table: "SalesPurchaseAllocations",
                column: "PurchaseCostCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPurchaseAllocations_QuantityUnitId",
                table: "SalesPurchaseAllocations",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPurchaseAllocations_SupplierPurchaseOrderLineId",
                table: "SalesPurchaseAllocations",
                column: "SupplierPurchaseOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTaskDefinitions_Claim",
                table: "ScheduledTaskDefinitions",
                columns: new[] { "Status", "NextRunAtUtc", "EndAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTaskDefinitions_Contact_Status_NextRunAtUtc",
                table: "ScheduledTaskDefinitions",
                columns: new[] { "ContactPointId", "Status", "NextRunAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTaskDefinitions_LockedUntilUtc",
                table: "ScheduledTaskDefinitions",
                column: "LockedUntilUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTaskDefinitions_Organisation_Status_NextRunAtUtc",
                table: "ScheduledTaskDefinitions",
                columns: new[] { "PartyId", "Status", "NextRunAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTaskDefinitions_PartyId_TaskType_IdempotencyKey",
                table: "ScheduledTaskDefinitions",
                columns: new[] { "PartyId", "TaskType", "IdempotencyKey" },
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTaskDefinitions_UserId",
                table: "ScheduledTaskDefinitions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_Claim",
                table: "ScheduledTasks",
                columns: new[] { "Status", "RunAtUtc", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_Contact_Status_RunAtUtc",
                table: "ScheduledTasks",
                columns: new[] { "ContactPointId", "Status", "RunAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_DefinitionId",
                table: "ScheduledTasks",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_HangfireJobId",
                table: "ScheduledTasks",
                column: "HangfireJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_LockedUntilUtc",
                table: "ScheduledTasks",
                column: "LockedUntilUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_Organisation_Status_RunAtUtc",
                table: "ScheduledTasks",
                columns: new[] { "PartyId", "Status", "RunAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTasks_UserId",
                table: "ScheduledTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_ScheduledTasks_Definition_Occurrence",
                table: "ScheduledTasks",
                columns: new[] { "DefinitionId", "IdempotencyKey" },
                unique: true,
                filter: "\"DefinitionId\" IS NOT NULL AND \"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreContributions_RequirementEvaluationId_SupplySolutionCa~",
                table: "ScoreContributions",
                columns: new[] { "RequirementEvaluationId", "SupplySolutionCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreContributions_SupplySolutionCandidateId_Category",
                table: "ScoreContributions",
                columns: new[] { "SupplySolutionCandidateId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDocuments_DocumentTypeId",
                table: "ShipmentDocuments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDocuments_ShipmentId_DocumentTypeId_Status",
                table: "ShipmentDocuments",
                columns: new[] { "ShipmentId", "DocumentTypeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDocuments_ShipmentLegId_ShipmentId",
                table: "ShipmentDocuments",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLegs_CarrierBusinessRelationshipId",
                table: "ShipmentLegs",
                column: "CarrierBusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLegs_FromAddressId",
                table: "ShipmentLegs",
                column: "FromAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLegs_FromLocationId",
                table: "ShipmentLegs",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLegs_ShipmentId_SequenceNumber",
                table: "ShipmentLegs",
                columns: new[] { "ShipmentId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLegs_ToAddressId",
                table: "ShipmentLegs",
                column: "ToAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLegs_ToLocationId",
                table: "ShipmentLegs",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLotAllocations_InventoryLotId",
                table: "ShipmentLotAllocations",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLotAllocations_QuantityUnitId",
                table: "ShipmentLotAllocations",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLotAllocations_ShipmentId_InventoryLotId",
                table: "ShipmentLotAllocations",
                columns: new[] { "ShipmentId", "InventoryLotId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentLotAllocations_ShipmentLegId_ShipmentId",
                table: "ShipmentLotAllocations",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentMilestones_ShipmentId_ShipmentMilestoneTypeId_Plann~",
                table: "ShipmentMilestones",
                columns: new[] { "ShipmentId", "ShipmentMilestoneTypeId", "PlannedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentMilestones_ShipmentLegId_ShipmentId",
                table: "ShipmentMilestones",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentMilestones_ShipmentMilestoneTypeId",
                table: "ShipmentMilestones",
                column: "ShipmentMilestoneTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentMilestoneTypes_Code",
                table: "ShipmentMilestoneTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentMilestoneTypes_IsActive_SortOrder",
                table: "ShipmentMilestoneTypes",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentOrderLineAllocations_BuyerSalesOrderLineId",
                table: "ShipmentOrderLineAllocations",
                column: "BuyerSalesOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentOrderLineAllocations_QuantityUnitId",
                table: "ShipmentOrderLineAllocations",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentOrderLineAllocations_ShipmentId_BuyerSalesOrderLine~",
                table: "ShipmentOrderLineAllocations",
                columns: new[] { "ShipmentId", "BuyerSalesOrderLineId", "SupplierPurchaseOrderLineId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentOrderLineAllocations_ShipmentLegId_ShipmentId",
                table: "ShipmentOrderLineAllocations",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentOrderLineAllocations_SupplierPurchaseOrderLineId",
                table: "ShipmentOrderLineAllocations",
                column: "SupplierPurchaseOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DestinationAddressId",
                table: "Shipments",
                column: "DestinationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DestinationLocationId",
                table: "Shipments",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_FreightForwarderBusinessRelationshipId",
                table: "Shipments",
                column: "FreightForwarderBusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OriginAddressId",
                table: "Shipments",
                column: "OriginAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OriginLocationId",
                table: "Shipments",
                column: "OriginLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentNumber",
                table: "Shipments",
                column: "ShipmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Status_PlannedPickupAtUtc_PlannedDeliveryAtUtc",
                table: "Shipments",
                columns: new[] { "Status", "PlannedPickupAtUtc", "PlannedDeliveryAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTrackingEvents_ExternalSystemId_ExternalEventId",
                table: "ShipmentTrackingEvents",
                columns: new[] { "ExternalSystemId", "ExternalEventId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTrackingEvents_LocationId",
                table: "ShipmentTrackingEvents",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTrackingEvents_ShipmentId_OccurredAtUtc",
                table: "ShipmentTrackingEvents",
                columns: new[] { "ShipmentId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTrackingEvents_ShipmentLegId_ShipmentId",
                table: "ShipmentTrackingEvents",
                columns: new[] { "ShipmentLegId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingBiologicalSources_BusinessRelation~1",
                table: "SupplierMaterialOfferingBiologicalSources",
                columns: new[] { "BusinessRelationshipBiologicalSourceClaimId", "BusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingBiologicalSources_BusinessRelations~",
                table: "SupplierMaterialOfferingBiologicalSources",
                column: "BusinessRelationshipBiologicalSourceClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingBiologicalSources_SupplierMaterial~1",
                table: "SupplierMaterialOfferingBiologicalSources",
                columns: new[] { "SupplierMaterialOfferingId", "BusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingBiologicalSources_SupplierMaterialO~",
                table: "SupplierMaterialOfferingBiologicalSources",
                columns: new[] { "SupplierMaterialOfferingId", "BusinessRelationshipBiologicalSourceClaimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingCharacteristics_CharacteristicId",
                table: "SupplierMaterialOfferingCharacteristics",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingCharacteristics_SupplierMaterialOff~",
                table: "SupplierMaterialOfferingCharacteristics",
                columns: new[] { "SupplierMaterialOfferingId", "CharacteristicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingEvidence_SupplierMaterialOfferingId~",
                table: "SupplierMaterialOfferingEvidence",
                columns: new[] { "SupplierMaterialOfferingId", "SourceSystem", "SourceRecordKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingOrigins_GeographicLocationId",
                table: "SupplierMaterialOfferingOrigins",
                column: "GeographicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingOrigins_SupplierMaterialOfferingId_~",
                table: "SupplierMaterialOfferingOrigins",
                columns: new[] { "SupplierMaterialOfferingId", "Role" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferingOrigins_SupplierMaterialOfferingId~1",
                table: "SupplierMaterialOfferingOrigins",
                columns: new[] { "SupplierMaterialOfferingId", "GeographicLocationId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferings_BusinessRelationshipId_Normalized~",
                table: "SupplierMaterialOfferings",
                columns: new[] { "BusinessRelationshipId", "NormalizedSupplierSku" },
                unique: true,
                filter: "\"NormalizedSupplierSku\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferings_BusinessRelationshipId_OfferingKey",
                table: "SupplierMaterialOfferings",
                columns: new[] { "BusinessRelationshipId", "OfferingKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferings_MappedMaterialId",
                table: "SupplierMaterialOfferings",
                column: "MappedMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferings_OfferedMaterialTypeId",
                table: "SupplierMaterialOfferings",
                column: "OfferedMaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterialOfferings_Status_LastObservedUtc",
                table: "SupplierMaterialOfferings",
                columns: new[] { "Status", "LastObservedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingAvailabilities_AvailabilityLocationId",
                table: "SupplierOfferingAvailabilities",
                column: "AvailabilityLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingAvailabilities_InventoryLotId",
                table: "SupplierOfferingAvailabilities",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingAvailabilities_MinimumOrderUnitId",
                table: "SupplierOfferingAvailabilities",
                column: "MinimumOrderUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingAvailabilities_QuantityUnitId",
                table: "SupplierOfferingAvailabilities",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingAvailabilities_SupplierMaterialOfferingId_S~",
                table: "SupplierOfferingAvailabilities",
                columns: new[] { "SupplierMaterialOfferingId", "Status", "AvailableFromUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingSpecifications_CharacteristicId",
                table: "SupplierOfferingSpecifications",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingSpecifications_ConstituentId",
                table: "SupplierOfferingSpecifications",
                column: "ConstituentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingSpecifications_SupplierMaterialOfferingId_K~",
                table: "SupplierOfferingSpecifications",
                columns: new[] { "SupplierMaterialOfferingId", "Kind", "ValidFromUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingSpecifications_SupplyDocumentId",
                table: "SupplierOfferingSpecifications",
                column: "SupplyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferingSpecifications_UnitOfMeasureId",
                table: "SupplierOfferingSpecifications",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilities_BusinessRelationshipId_Capabili~",
                table: "SupplierProcessCapabilities",
                columns: new[] { "BusinessRelationshipId", "CapabilityKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilities_InputMaterialTypeId",
                table: "SupplierProcessCapabilities",
                column: "InputMaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilities_OutputMaterialTypeId",
                table: "SupplierProcessCapabilities",
                column: "OutputMaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilities_ProcessTypeId",
                table: "SupplierProcessCapabilities",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilities_Status_VerificationStatus",
                table: "SupplierProcessCapabilities",
                columns: new[] { "Status", "VerificationStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilityBiologicalSourceScopes_BusinessRe~1",
                table: "SupplierProcessCapabilityBiologicalSourceScopes",
                columns: new[] { "BusinessRelationshipBiologicalSourceClaimId", "BusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilityBiologicalSourceScopes_BusinessRel~",
                table: "SupplierProcessCapabilityBiologicalSourceScopes",
                column: "BusinessRelationshipBiologicalSourceClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilityBiologicalSourceScopes_SupplierPr~1",
                table: "SupplierProcessCapabilityBiologicalSourceScopes",
                columns: new[] { "SupplierProcessCapabilityId", "BusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilityBiologicalSourceScopes_SupplierPro~",
                table: "SupplierProcessCapabilityBiologicalSourceScopes",
                columns: new[] { "SupplierProcessCapabilityId", "BusinessRelationshipBiologicalSourceClaimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProcessCapabilityEvidence_SupplierProcessCapability~",
                table: "SupplierProcessCapabilityEvidence",
                columns: new[] { "SupplierProcessCapabilityId", "SourceSystem", "SourceRecordKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_InventoryLotId_SupplierBusinessR~",
                table: "SupplierPurchaseOrderLines",
                columns: new[] { "InventoryLotId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_MaterialId",
                table: "SupplierPurchaseOrderLines",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_QuantityUnitId",
                table: "SupplierPurchaseOrderLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_SupplierMaterialOfferingId_Suppl~",
                table: "SupplierPurchaseOrderLines",
                columns: new[] { "SupplierMaterialOfferingId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_SupplierProcessCapabilityId_Supp~",
                table: "SupplierPurchaseOrderLines",
                columns: new[] { "SupplierProcessCapabilityId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_SupplierPurchaseOrderId_LineNumb~",
                table: "SupplierPurchaseOrderLines",
                columns: new[] { "SupplierPurchaseOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_SupplierPurchaseOrderId_Supplier~",
                table: "SupplierPurchaseOrderLines",
                columns: new[] { "SupplierPurchaseOrderId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_SupplierQuoteLineId_SupplierBusi~",
                table: "SupplierPurchaseOrderLines",
                columns: new[] { "SupplierQuoteLineId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderLines_SupplySolutionStepId",
                table: "SupplierPurchaseOrderLines",
                column: "SupplySolutionStepId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrders_CurrencyId",
                table: "SupplierPurchaseOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrders_IncotermRuleId",
                table: "SupplierPurchaseOrders",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrders_OrderNumber",
                table: "SupplierPurchaseOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrders_SupplierBusinessRelationshipId_Status",
                table: "SupplierPurchaseOrders",
                columns: new[] { "SupplierBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrders_SupplierQuoteRevisionId_SupplierBusi~",
                table: "SupplierPurchaseOrders",
                columns: new[] { "SupplierQuoteRevisionId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPurchaseOrderStatusHistory_SupplierPurchaseOrderId_~",
                table: "SupplierPurchaseOrderStatusHistory",
                columns: new[] { "SupplierPurchaseOrderId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_BuyerRfqLineId",
                table: "SupplierQuoteLines",
                column: "BuyerRfqLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_IncotermRuleId",
                table: "SupplierQuoteLines",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_InventoryLotId_SupplierBusinessRelations~",
                table: "SupplierQuoteLines",
                columns: new[] { "InventoryLotId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_QuantityUnitId",
                table: "SupplierQuoteLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_SupplierMaterialOfferingId_SupplierBusin~",
                table: "SupplierQuoteLines",
                columns: new[] { "SupplierMaterialOfferingId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_SupplierMaterialOfferingId_SupplierProce~",
                table: "SupplierQuoteLines",
                columns: new[] { "SupplierMaterialOfferingId", "SupplierProcessCapabilityId", "SupplySolutionStepId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_SupplierProcessCapabilityId_SupplierBusi~",
                table: "SupplierQuoteLines",
                columns: new[] { "SupplierProcessCapabilityId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_SupplierQuoteRevisionId_LineNumber",
                table: "SupplierQuoteLines",
                columns: new[] { "SupplierQuoteRevisionId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_SupplierQuoteRevisionId_SupplierBusiness~",
                table: "SupplierQuoteLines",
                columns: new[] { "SupplierQuoteRevisionId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteLines_SupplySolutionStepId",
                table: "SupplierQuoteLines",
                column: "SupplySolutionStepId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteRevisions_CurrencyId",
                table: "SupplierQuoteRevisions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteRevisions_IncotermRuleId",
                table: "SupplierQuoteRevisions",
                column: "IncotermRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteRevisions_SnapshotHash",
                table: "SupplierQuoteRevisions",
                column: "SnapshotHash");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteRevisions_SupplierQuoteId_RevisionNumber",
                table: "SupplierQuoteRevisions",
                columns: new[] { "SupplierQuoteId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuoteRevisions_SupplierQuoteId_SupplierBusinessRela~",
                table: "SupplierQuoteRevisions",
                columns: new[] { "SupplierQuoteId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "UX_SupplierQuoteRevisions_OneCurrentCommercialRevision",
                table: "SupplierQuoteRevisions",
                column: "SupplierQuoteId",
                unique: true,
                filter: "\"Status\" IN ('Received', 'AcceptedInternally')");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotes_BuyerRfqRevisionId",
                table: "SupplierQuotes",
                column: "BuyerRfqRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotes_QuoteNumber",
                table: "SupplierQuotes",
                column: "QuoteNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotes_SupplierBusinessRelationshipId_Status",
                table: "SupplierQuotes",
                columns: new[] { "SupplierBusinessRelationshipId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_InventoryLotId_SupplierBusinessRelationsh~",
                table: "SupplyAllocations",
                columns: new[] { "InventoryLotId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_PurchasePriceCurrencyId",
                table: "SupplyAllocations",
                column: "PurchasePriceCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_QuantityUnitId",
                table: "SupplyAllocations",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_SupplierBusinessRelationshipId_SupplierMa~",
                table: "SupplyAllocations",
                columns: new[] { "SupplierBusinessRelationshipId", "SupplierMaterialOfferingId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_SupplierMaterialOfferingId_SupplierBusine~",
                table: "SupplyAllocations",
                columns: new[] { "SupplierMaterialOfferingId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_SupplierOfferingAvailabilityId_SupplierMa~",
                table: "SupplyAllocations",
                columns: new[] { "SupplierOfferingAvailabilityId", "SupplierMaterialOfferingId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_SupplierQuoteLineId_SupplierBusinessRelat~",
                table: "SupplyAllocations",
                columns: new[] { "SupplierQuoteLineId", "SupplierBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAllocations_SupplySolutionLineId_AllocationSequence",
                table: "SupplyAllocations",
                columns: new[] { "SupplySolutionLineId", "AllocationSequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalyses_InventoryLotId_Status_ReportedAtUtc",
                table: "SupplyAnalyses",
                columns: new[] { "InventoryLotId", "Status", "ReportedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalyses_LaboratoryBusinessRelationshipId",
                table: "SupplyAnalyses",
                column: "LaboratoryBusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalyses_SupplierMaterialOfferingId_Status",
                table: "SupplyAnalyses",
                columns: new[] { "SupplierMaterialOfferingId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalyses_SupplyDocumentId",
                table: "SupplyAnalyses",
                column: "SupplyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalysisResults_CharacteristicId",
                table: "SupplyAnalysisResults",
                column: "CharacteristicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalysisResults_ConstituentId",
                table: "SupplyAnalysisResults",
                column: "ConstituentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalysisResults_SupplyAnalysisId_ConstituentId",
                table: "SupplyAnalysisResults",
                columns: new[] { "SupplyAnalysisId", "ConstituentId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyAnalysisResults_UnitOfMeasureId",
                table: "SupplyAnalysisResults",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_CertificationSchemeId_Status_ValidToUtc",
                table: "SupplyCertifications",
                columns: new[] { "CertificationSchemeId", "Status", "ValidToUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_CertificationSchemeJurisdictionId_Cert~",
                table: "SupplyCertifications",
                columns: new[] { "CertificationSchemeJurisdictionId", "CertificationSchemeId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_CertifiedBusinessRelationshipId_Suppli~",
                table: "SupplyCertifications",
                columns: new[] { "CertifiedBusinessRelationshipId", "SupplierMaterialOfferingId", "SupplierProcessCapabilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_CertifiedSiteLocationId",
                table: "SupplyCertifications",
                column: "CertifiedSiteLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_InventoryLotId_ShipmentId",
                table: "SupplyCertifications",
                columns: new[] { "InventoryLotId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_IssuerBusinessRelationshipId",
                table: "SupplyCertifications",
                column: "IssuerBusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_ShipmentId",
                table: "SupplyCertifications",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_SupplierMaterialOfferingId",
                table: "SupplyCertifications",
                column: "SupplierMaterialOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_SupplierProcessCapabilityId",
                table: "SupplyCertifications",
                column: "SupplierProcessCapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCertifications_SupplyDocumentId",
                table: "SupplyCertifications",
                column: "SupplyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_ContentHash",
                table: "SupplyDocuments",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_DocumentTypeId_Status_ValidToUtc",
                table: "SupplyDocuments",
                columns: new[] { "DocumentTypeId", "Status", "ValidToUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_InventoryLotId",
                table: "SupplyDocuments",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_ReferenceEntryId",
                table: "SupplyDocuments",
                column: "ReferenceEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_ShipmentId",
                table: "SupplyDocuments",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_SubjectBusinessRelationshipId_SupplierMater~",
                table: "SupplyDocuments",
                columns: new[] { "SubjectBusinessRelationshipId", "SupplierMaterialOfferingId", "SupplierProcessCapabilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_SupplierMaterialOfferingId",
                table: "SupplyDocuments",
                column: "SupplierMaterialOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyDocuments_SupplierProcessCapabilityId",
                table: "SupplyDocuments",
                column: "SupplierProcessCapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionCandidates_EstimatedCostCurrencyId",
                table: "SupplySolutionCandidates",
                column: "EstimatedCostCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionCandidates_MatchingRunId_CandidateNumber",
                table: "SupplySolutionCandidates",
                columns: new[] { "MatchingRunId", "CandidateNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionCandidates_SnapshotHash",
                table: "SupplySolutionCandidates",
                column: "SnapshotHash");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionCandidates_Status_ExpiresAtUtc",
                table: "SupplySolutionCandidates",
                columns: new[] { "Status", "ExpiresAtUtc" },
                filter: "\"ExpiresAtUtc\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionCandidates_Status_Outcome_TotalScore",
                table: "SupplySolutionCandidates",
                columns: new[] { "Status", "Outcome", "TotalScore" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionLines_MatchingRunLineId_MatchingRunId",
                table: "SupplySolutionLines",
                columns: new[] { "MatchingRunLineId", "MatchingRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionLines_QuantityUnitId",
                table: "SupplySolutionLines",
                column: "QuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionLines_SupplySolutionCandidateId_MatchingRunId",
                table: "SupplySolutionLines",
                columns: new[] { "SupplySolutionCandidateId", "MatchingRunId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionLines_SupplySolutionCandidateId_MatchingRunLi~",
                table: "SupplySolutionLines",
                columns: new[] { "SupplySolutionCandidateId", "MatchingRunLineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionStepDependencies_PredecessorStepId_SuccessorS~",
                table: "SupplySolutionStepDependencies",
                columns: new[] { "PredecessorStepId", "SuccessorStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionStepDependencies_PredecessorStepId_SupplySolu~",
                table: "SupplySolutionStepDependencies",
                columns: new[] { "PredecessorStepId", "SupplySolutionCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionStepDependencies_SuccessorStepId_SupplySoluti~",
                table: "SupplySolutionStepDependencies",
                columns: new[] { "SuccessorStepId", "SupplySolutionCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_FromLocationId",
                table: "SupplySolutionSteps",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_InputMaterialId",
                table: "SupplySolutionSteps",
                column: "InputMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_InputQuantityUnitId",
                table: "SupplySolutionSteps",
                column: "InputQuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_OutputMaterialId",
                table: "SupplySolutionSteps",
                column: "OutputMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_OutputQuantityUnitId",
                table: "SupplySolutionSteps",
                column: "OutputQuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_ProcessTypeId",
                table: "SupplySolutionSteps",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_ProviderBusinessRelationshipId",
                table: "SupplySolutionSteps",
                column: "ProviderBusinessRelationshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_SupplierProcessCapabilityId_ProviderBus~",
                table: "SupplySolutionSteps",
                columns: new[] { "SupplierProcessCapabilityId", "ProviderBusinessRelationshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_SupplySolutionCandidateId_SequenceNumber",
                table: "SupplySolutionSteps",
                columns: new[] { "SupplySolutionCandidateId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_SupplySolutionLineId_SupplySolutionCand~",
                table: "SupplySolutionSteps",
                columns: new[] { "SupplySolutionLineId", "SupplySolutionCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplySolutionSteps_ToLocationId",
                table: "SupplySolutionSteps",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxon_AcceptedTaxonId",
                table: "Taxon",
                column: "AcceptedTaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxon_ParentTaxonId_Rank_NormalizedName",
                table: "Taxon",
                columns: new[] { "ParentTaxonId", "Rank", "NormalizedName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Taxon_PathKey",
                table: "Taxon",
                column: "PathKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_BuyerProposalAcceptanceId",
                table: "TradeExecutionCheckpoints",
                column: "BuyerProposalAcceptanceId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_BuyerSalesOrderId_SupplierPurchas~",
                table: "TradeExecutionCheckpoints",
                columns: new[] { "BuyerSalesOrderId", "SupplierPurchaseOrderId", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_CommercialInvoiceId_Status",
                table: "TradeExecutionCheckpoints",
                columns: new[] { "CommercialInvoiceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_ExternalSystemId",
                table: "TradeExecutionCheckpoints",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_GoodsResponsibilityTransferId",
                table: "TradeExecutionCheckpoints",
                column: "GoodsResponsibilityTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_ParentCheckpointId",
                table: "TradeExecutionCheckpoints",
                column: "ParentCheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_ShipmentId",
                table: "TradeExecutionCheckpoints",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_Status_DueAtUtc_OwnerUserId",
                table: "TradeExecutionCheckpoints",
                columns: new[] { "Status", "DueAtUtc", "OwnerUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_SupplierPurchaseOrderId",
                table: "TradeExecutionCheckpoints",
                column: "SupplierPurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_SupplySolutionStepId",
                table: "TradeExecutionCheckpoints",
                column: "SupplySolutionStepId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpoints_TradeExecutionCheckpointTypeId",
                table: "TradeExecutionCheckpoints",
                column: "TradeExecutionCheckpointTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpointTypes_Code",
                table: "TradeExecutionCheckpointTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeExecutionCheckpointTypes_IsActive_DefaultSortOrder",
                table: "TradeExecutionCheckpointTypes",
                columns: new[] { "IsActive", "DefaultSortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_Code",
                table: "UnitsOfMeasure",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_Code_Dimension_SortOrder",
                table: "UnitsOfMeasure",
                columns: new[] { "Code", "Dimension", "SortOrder" },
                unique: true,
                filter: "\"IsActive\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "User",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId_ClaimType_ClaimValue",
                table: "UserClaim",
                columns: new[] { "UserId", "ClaimType", "ClaimValue" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Waters_ParentWaterId",
                table: "Waters",
                column: "ParentWaterId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerHeartbeats_LastSeenUtc",
                table: "WorkerHeartbeats",
                column: "LastSeenUtc");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerHeartbeats_ServiceName",
                table: "WorkerHeartbeats",
                column: "ServiceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BiologicalSourceCharacteristics");

            migrationBuilder.DropTable(
                name: "BiologicalSourceConstituents");

            migrationBuilder.DropTable(
                name: "BiologicalSourceNames");

            migrationBuilder.DropTable(
                name: "BiologicalSourceReferences");

            migrationBuilder.DropTable(
                name: "BiologicalSourceRegions");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipBiologicalSourceClaimEvidence");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipBiologicalSourceClaimRegions");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipContacts");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipStageHistory");

            migrationBuilder.DropTable(
                name: "BuyerDemandStateHistory");

            migrationBuilder.DropTable(
                name: "BuyerProposalRevisionScheduledTaskLinks");

            migrationBuilder.DropTable(
                name: "BuyerRfqDeliveryLineAllocations");

            migrationBuilder.DropTable(
                name: "BuyerRfqLineMaterialOptions");

            migrationBuilder.DropTable(
                name: "BuyerRfqScheduledTaskLinks");

            migrationBuilder.DropTable(
                name: "BuyerSalesOrderDeliveryLines");

            migrationBuilder.DropTable(
                name: "BuyerSalesOrderStatusHistory");

            migrationBuilder.DropTable(
                name: "CapacityEvaluations");

            migrationBuilder.DropTable(
                name: "CarrierBookings");

            migrationBuilder.DropTable(
                name: "CommercialInvoiceLines");

            migrationBuilder.DropTable(
                name: "CountryWater");

            migrationBuilder.DropTable(
                name: "CrmActivityParticipants");

            migrationBuilder.DropTable(
                name: "CrmActivityScheduledTaskLinks");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRateChanges");

            migrationBuilder.DropTable(
                name: "CustomsReferences");

            migrationBuilder.DropTable(
                name: "DataMigration");

            migrationBuilder.DropTable(
                name: "DisclosurePackageItems");

            migrationBuilder.DropTable(
                name: "EmailOutboxRecipients");

            migrationBuilder.DropTable(
                name: "ExternalAccountingReferences");

            migrationBuilder.DropTable(
                name: "ExternalMessageAttachments");

            migrationBuilder.DropTable(
                name: "InventoryLotLinks");

            migrationBuilder.DropTable(
                name: "InventoryLotOrigins");

            migrationBuilder.DropTable(
                name: "InvoiceSyncAttempts");

            migrationBuilder.DropTable(
                name: "MatchingModerationDecisions");

            migrationBuilder.DropTable(
                name: "MaterialBiologicalSources");

            migrationBuilder.DropTable(
                name: "MaterialCharacteristics");

            migrationBuilder.DropTable(
                name: "MaterialConstituents");

            migrationBuilder.DropTable(
                name: "MaterialOrigins");

            migrationBuilder.DropTable(
                name: "MaterialReferences");

            migrationBuilder.DropTable(
                name: "MaterialTransformationInputs");

            migrationBuilder.DropTable(
                name: "MaterialTransformationOutputs");

            migrationBuilder.DropTable(
                name: "OrganizationIdentifiers");

            migrationBuilder.DropTable(
                name: "OrganizationPersonFunctions");

            migrationBuilder.DropTable(
                name: "PartyAddresses");

            migrationBuilder.DropTable(
                name: "PartyExternalReferences");

            migrationBuilder.DropTable(
                name: "PaymentStatusSnapshots");

            migrationBuilder.DropTable(
                name: "PersonUserAccounts");

            migrationBuilder.DropTable(
                name: "ProductCharacteristics");

            migrationBuilder.DropTable(
                name: "ProductIdentifiers");

            migrationBuilder.DropTable(
                name: "ProductReferences");

            migrationBuilder.DropTable(
                name: "ProviderEmailTemplateMapping");

            migrationBuilder.DropTable(
                name: "ReconciliationTasks");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "SalesPurchaseAllocations");

            migrationBuilder.DropTable(
                name: "ScoreContributions");

            migrationBuilder.DropTable(
                name: "SecurityKeys");

            migrationBuilder.DropTable(
                name: "ShipmentLotAllocations");

            migrationBuilder.DropTable(
                name: "ShipmentMilestones");

            migrationBuilder.DropTable(
                name: "ShipmentOrderLineAllocations");

            migrationBuilder.DropTable(
                name: "ShipmentTrackingEvents");

            migrationBuilder.DropTable(
                name: "SupplierMaterialOfferingBiologicalSources");

            migrationBuilder.DropTable(
                name: "SupplierMaterialOfferingCharacteristics");

            migrationBuilder.DropTable(
                name: "SupplierMaterialOfferingEvidence");

            migrationBuilder.DropTable(
                name: "SupplierMaterialOfferingOrigins");

            migrationBuilder.DropTable(
                name: "SupplierOfferingSpecifications");

            migrationBuilder.DropTable(
                name: "SupplierProcessCapabilityBiologicalSourceScopes");

            migrationBuilder.DropTable(
                name: "SupplierProcessCapabilityEvidence");

            migrationBuilder.DropTable(
                name: "SupplierPurchaseOrderStatusHistory");

            migrationBuilder.DropTable(
                name: "SupplyAnalysisResults");

            migrationBuilder.DropTable(
                name: "SupplyCertifications");

            migrationBuilder.DropTable(
                name: "SupplySolutionStepDependencies");

            migrationBuilder.DropTable(
                name: "TradeExecutionCheckpoints");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "WorkerHeartbeats");

            migrationBuilder.DropTable(
                name: "BuyerDemandEvidence");

            migrationBuilder.DropTable(
                name: "BuyerRfqDeliveries");

            migrationBuilder.DropTable(
                name: "BuyerSalesOrderDeliveries");

            migrationBuilder.DropTable(
                name: "ShipmentDocuments");

            migrationBuilder.DropTable(
                name: "EmailOutboxMessages");

            migrationBuilder.DropTable(
                name: "ExternalMessages");

            migrationBuilder.DropTable(
                name: "MaterialTransformations");

            migrationBuilder.DropTable(
                name: "ContactFunctionTypes");

            migrationBuilder.DropTable(
                name: "EmailTemplateVariant");

            migrationBuilder.DropTable(
                name: "RequirementEvaluations");

            migrationBuilder.DropTable(
                name: "ShipmentMilestoneTypes");

            migrationBuilder.DropTable(
                name: "BuyerSalesOrderLines");

            migrationBuilder.DropTable(
                name: "SupplierPurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "SupplyAnalyses");

            migrationBuilder.DropTable(
                name: "CommercialInvoices");

            migrationBuilder.DropTable(
                name: "GoodsResponsibilityTransfers");

            migrationBuilder.DropTable(
                name: "TradeExecutionCheckpointTypes");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "CrmActivities");

            migrationBuilder.DropTable(
                name: "ScheduledTasks");

            migrationBuilder.DropTable(
                name: "ExternalConversations");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "BuyerRfqRequirements");

            migrationBuilder.DropTable(
                name: "SupplyAllocations");

            migrationBuilder.DropTable(
                name: "BuyerProposalAcceptanceLines");

            migrationBuilder.DropTable(
                name: "ExternalSystems");

            migrationBuilder.DropTable(
                name: "ShipmentLegs");

            migrationBuilder.DropTable(
                name: "SupplyDocuments");

            migrationBuilder.DropTable(
                name: "ScheduledTaskDefinitions");

            migrationBuilder.DropTable(
                name: "BuyerSalesOrders");

            migrationBuilder.DropTable(
                name: "SupplierPurchaseOrders");

            migrationBuilder.DropTable(
                name: "CertificationSchemeJurisdictions");

            migrationBuilder.DropTable(
                name: "Constituents");

            migrationBuilder.DropTable(
                name: "SupplierOfferingAvailabilities");

            migrationBuilder.DropTable(
                name: "SupplierQuoteLines");

            migrationBuilder.DropTable(
                name: "BuyerProposalLines");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "ReferenceEntries");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "PartyContactPoints");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "BuyerProposalAcceptances");

            migrationBuilder.DropTable(
                name: "CertificationSchemes");

            migrationBuilder.DropTable(
                name: "ConstituentTypes");

            migrationBuilder.DropTable(
                name: "InventoryLots");

            migrationBuilder.DropTable(
                name: "SupplierQuoteRevisions");

            migrationBuilder.DropTable(
                name: "SupplySolutionSteps");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "OrganizationPeople");

            migrationBuilder.DropTable(
                name: "BuyerProposalOptions");

            migrationBuilder.DropTable(
                name: "SupplierMaterialOfferings");

            migrationBuilder.DropTable(
                name: "IncotermRules");

            migrationBuilder.DropTable(
                name: "SupplierQuotes");

            migrationBuilder.DropTable(
                name: "GeographicLocations");

            migrationBuilder.DropTable(
                name: "SupplierProcessCapabilities");

            migrationBuilder.DropTable(
                name: "SupplySolutionLines");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "BuyerProposalRevisions");

            migrationBuilder.DropTable(
                name: "Waters");

            migrationBuilder.DropTable(
                name: "ProcessTypes");

            migrationBuilder.DropTable(
                name: "MatchingRunLines");

            migrationBuilder.DropTable(
                name: "SupplySolutionCandidates");

            migrationBuilder.DropTable(
                name: "BuyerProposals");

            migrationBuilder.DropTable(
                name: "DisclosurePackageRevisions");

            migrationBuilder.DropTable(
                name: "BuyerRfqLines");

            migrationBuilder.DropTable(
                name: "MatchingRuns");

            migrationBuilder.DropTable(
                name: "DisclosurePackages");

            migrationBuilder.DropTable(
                name: "BuyerDemandScopes");

            migrationBuilder.DropTable(
                name: "BuyerRfqRevisions");

            migrationBuilder.DropTable(
                name: "MatchingRuleSetVersions");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipBiologicalSourceClaims");

            migrationBuilder.DropTable(
                name: "Characteristics");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasure");

            migrationBuilder.DropTable(
                name: "BuyerRfqs");

            migrationBuilder.DropTable(
                name: "MatchingRuleSets");

            migrationBuilder.DropTable(
                name: "BiologicalSources");

            migrationBuilder.DropTable(
                name: "CharacteristicTypes");

            migrationBuilder.DropTable(
                name: "MaterialTypes");

            migrationBuilder.DropTable(
                name: "BuyerDemands");

            migrationBuilder.DropTable(
                name: "Taxon");

            migrationBuilder.DropTable(
                name: "BusinessRelationships");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipStages");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "BusinessRelationshipTypes");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropTable(
                name: "Continents");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
