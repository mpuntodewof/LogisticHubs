using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IdempotencyRecords",
                columns: table => new
                {
                    IdempotencyKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Endpoint = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    ResponseBody = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRecords", x => x.IdempotencyKey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "text", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Exception = table.Column<string>(type: "text", maxLength: 8000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestPath = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestMethod = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AdditionalDataJson = table.Column<string>(type: "text", maxLength: 8000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "text", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Group = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsReadOnly = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UserEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    OldValuesJson = table.Column<string>(type: "text", maxLength: 8000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValuesJson = table.Column<string>(type: "text", maxLength: 8000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalInfo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brands_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCategoryId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChartOfAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AccountCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountSubType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentAccountId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsSystemAccount = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    NormalBalance = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartOfAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartOfAccounts_ChartOfAccounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartOfAccounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EntryNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reference = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceDocumentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceDocumentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VoidedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VoidReason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntries_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DueDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTerms_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Resource = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSystem = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxRates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "text", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Group = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsReadOnly = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Abbreviation = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitsOfMeasure_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    JournalEntryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AccountId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_ChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvoiceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxInvoiceNumber = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceDocumentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceDocumentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ReferenceDocumentNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CounterpartyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentTermId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IssuedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancellationReason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_PaymentTerms_PaymentTermId",
                        column: x => x.PaymentTermId,
                        principalTable: "PaymentTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    BrandId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    BaseUnitOfMeasureId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_UnitsOfMeasure_BaseUnitOfMeasureId",
                        column: x => x.BaseUnitOfMeasureId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UnitConversions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FromUnitId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ToUnitId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitConversions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitConversions_UnitsOfMeasure_FromUnitId",
                        column: x => x.FromUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitConversions_UnitsOfMeasure_ToUnitId",
                        column: x => x.ToUnitId,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TokenHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByIp = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedByIp = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReplacedByToken = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductTaxRates",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TaxRateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTaxRates", x => new { x.ProductId, x.TaxRateId });
                    table.ForeignKey(
                        name: "FK_ProductTaxRates_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTaxRates_TaxRates_TaxRateId",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Sku = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VariantAttributes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Barcode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvoiceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductVariantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VariantName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sku = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRateId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TaxRateValue = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_TaxRates_TaxRateId",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductVariantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ImageUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AltText = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductImages_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WarehouseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductVariantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MovementType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reason = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    QuantityBefore = table.Column<int>(type: "int", nullable: false),
                    QuantityAfter = table.Column<int>(type: "int", nullable: false),
                    ReferenceDocumentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceDocumentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ReferenceDocumentNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceWarehouseId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DestinationWarehouseId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_SourceWarehouseId",
                        column: x => x.SourceWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WarehouseStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WarehouseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductVariantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    QuantityReserved = table.Column<int>(type: "int", nullable: false),
                    ReorderPoint = table.Column<int>(type: "int", nullable: true),
                    MaxStock = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseStocks", x => x.Id);
                    table.CheckConstraint("CK_WarehouseStock_NonNegative", "QuantityOnHand >= 0");
                    table.ForeignKey(
                        name: "FK_WarehouseStocks_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseStocks_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseStocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "CompanyName", "CreatedAt", "CreatedBy", "IsActive", "Slug", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "NiagaOne Default", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "default", null, null });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "CreatedBy", "Name", "Resource", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0001-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6049), null, "users.create", "users", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6064), null, "users.read", "users", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0003-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6069), null, "users.update", "users", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0004-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6083), null, "users.delete", "users", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0005-aaaa-aaaa-aaaaaaaaaaaa"), "assign", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6087), null, "roles.assign", "roles", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0006-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6093), null, "roles.create", "roles", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0007-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6098), null, "roles.read", "roles", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0008-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6177), null, "roles.update", "roles", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0009-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6183), null, "roles.delete", "roles", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6190), null, "categories.create", "categories", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6195), null, "categories.read", "categories", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6200), null, "categories.update", "categories", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6204), null, "categories.delete", "categories", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6209), null, "brands.create", "brands", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6213), null, "brands.read", "brands", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6218), null, "brands.update", "brands", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6222), null, "brands.delete", "brands", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6246), null, "units.create", "units", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6251), null, "units.read", "units", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6256), null, "units.update", "units", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6260), null, "units.delete", "units", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6264), null, "products.create", "products", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6269), null, "products.read", "products", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6280), null, "products.update", "products", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6296), null, "products.delete", "products", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6301), null, "inventory.read", "inventory", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6305), null, "inventory.create", "inventory", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6309), null, "inventory.update", "inventory", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"), "transfer", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6363), null, "inventory.transfer", "inventory", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"), "manage", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6401), null, "warehouses.manage", "warehouses", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6406), null, "chart-of-accounts.create", "chart-of-accounts", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6411), null, "chart-of-accounts.read", "chart-of-accounts", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6415), null, "chart-of-accounts.update", "chart-of-accounts", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6421), null, "chart-of-accounts.delete", "chart-of-accounts", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6425), null, "journal-entries.create", "journal-entries", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6429), null, "journal-entries.read", "journal-entries", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"), "post", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6434), null, "journal-entries.post", "journal-entries", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"), "void", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6450), null, "journal-entries.void", "journal-entries", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6454), null, "journal-entries.delete", "journal-entries", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6458), null, "payment-terms.create", "payment-terms", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6463), null, "payment-terms.read", "payment-terms", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6467), null, "payment-terms.update", "payment-terms", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6471), null, "payment-terms.delete", "payment-terms", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6507), null, "tax-rates.create", "tax-rates", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6512), null, "tax-rates.read", "tax-rates", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6516), null, "tax-rates.update", "tax-rates", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6521), null, "tax-rates.delete", "tax-rates", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"), "assign", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6525), null, "tax-rates.assign", "tax-rates", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"), "create", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6528), null, "invoices.create", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6533), null, "invoices.read", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"), "issue", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6537), null, "invoices.issue", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"), "assign-tax-number", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6542), null, "invoices.assign-tax-number", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"), "pay", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6547), null, "invoices.pay", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"), "cancel", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6551), null, "invoices.cancel", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"), "delete", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6555), null, "invoices.delete", "invoices", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6562), null, "audit-logs.read", "audit-logs", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"), "export", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6566), null, "audit-logs.export", "audit-logs", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0058-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6570), null, "system-logs.read", "system-logs", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6574), null, "tenant-settings.read", "tenant-settings", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6579), null, "tenant-settings.update", "tenant-settings", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0061-aaaa-aaaa-aaaaaaaaaaaa"), "read", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6583), null, "system-settings.read", "system-settings", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("aaaaaaaa-0062-aaaa-aaaa-aaaaaaaaaaaa"), "update", new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6588), null, "system-settings.update", "system-settings", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsSystem", "Name", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5894), null, null, null, "Full system access", true, "Admin", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5900), null, null, null, "Full operational access across all modules", true, "Manager", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5903), null, null, null, "Read-only access across all modules", true, "Viewer", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5905), null, null, null, "Inventory management, stock movements, receiving", true, "Warehouse Staff", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5907), null, null, null, "Finance, tax, invoicing, and payment management", true, "Accountant", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "IsActive", "Name", "PasswordHash", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "admin@niagaone.com", true, "Alice Admin", "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "manager@niagaone.com", true, "Marcus Manager", "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "viewer@niagaone.com", true, "Victor Viewer", "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "warehouse@niagaone.com", true, "Wira Warehouse", "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "accountant@niagaone.com", true, "Andi Accountant", "$2a$12$/1CjZqaBIZTbTErAYBktTuw/iK9Y1I.BYKu7J1B9ZWSh5KQhFw9Gy", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0001-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0003-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0004-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0005-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0006-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0007-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0008-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0009-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0058-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0061-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0062-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("77777777-7777-7777-7777-777777777777"), new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt", "AssignedBy", "TenantId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId",
                table: "AuditLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "TenantId", "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_Timestamp",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_TenantId",
                table: "Brands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_TenantId_Slug",
                table: "Brands",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId",
                table: "Categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId_Slug",
                table: "Categories",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_ParentAccountId",
                table: "ChartOfAccounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_TenantId",
                table: "ChartOfAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartOfAccounts_TenantId_AccountCode",
                table: "ChartOfAccounts",
                columns: new[] { "TenantId", "AccountCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_ExpiresAt",
                table: "IdempotencyRecords",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ProductVariantId",
                table: "InvoiceItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_TaxRateId",
                table: "InvoiceItems",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_TenantId",
                table: "InvoiceItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PaymentTermId",
                table: "Invoices",
                column: "PaymentTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TenantId",
                table: "Invoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TenantId_InvoiceNumber",
                table: "Invoices",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId",
                table: "JournalEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_EntryNumber",
                table: "JournalEntries",
                columns: new[] { "TenantId", "EntryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountId",
                table: "JournalEntryLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryId",
                table: "JournalEntryLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_TenantId",
                table: "JournalEntryLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_TenantId",
                table: "PaymentTerms",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_TenantId_Code",
                table: "PaymentTerms",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_TenantId",
                table: "Permissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_TenantId_Name",
                table: "Permissions",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_TenantId_Resource_Action",
                table: "Permissions",
                columns: new[] { "TenantId", "Resource", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductVariantId",
                table: "ProductImages",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_TenantId",
                table: "ProductImages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BaseUnitOfMeasureId",
                table: "Products",
                column: "BaseUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_Slug",
                table: "Products",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTaxRates_TaxRateId",
                table: "ProductTaxRates",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTaxRates_TenantId",
                table: "ProductTaxRates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_TenantId",
                table: "ProductVariants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_TenantId_Sku",
                table: "ProductVariants",
                columns: new[] { "TenantId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TenantId",
                table: "RefreshTokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_RevokedAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_TenantId",
                table: "RolePermissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId",
                table: "Roles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_Name",
                table: "Roles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DestinationWarehouseId",
                table: "StockMovements",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductVariantId",
                table: "StockMovements",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SourceWarehouseId",
                table: "StockMovements",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId",
                table: "StockMovements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_WarehouseId_ProductVariantId",
                table: "StockMovements",
                columns: new[] { "TenantId", "WarehouseId", "ProductVariantId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseId",
                table: "StockMovements",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_Level_Timestamp",
                table: "SystemLogs",
                columns: new[] { "Level", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_TenantId",
                table: "SystemLogs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_TenantId",
                table: "TaxRates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_TenantId_Code",
                table: "TaxRates",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId",
                table: "TenantSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId_Key",
                table: "TenantSettings",
                columns: new[] { "TenantId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversions_FromUnitId",
                table: "UnitConversions",
                column: "FromUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversions_TenantId",
                table: "UnitConversions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversions_TenantId_FromUnitId_ToUnitId",
                table: "UnitConversions",
                columns: new[] { "TenantId", "FromUnitId", "ToUnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversions_ToUnitId",
                table: "UnitConversions",
                column: "ToUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_TenantId",
                table: "UnitsOfMeasure",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_TenantId_Abbreviation",
                table: "UnitsOfMeasure",
                columns: new[] { "TenantId", "Abbreviation" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId",
                table: "UserRoles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_Email",
                table: "Users",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStocks_ProductVariantId",
                table: "WarehouseStocks",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStocks_TenantId",
                table: "WarehouseStocks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStocks_TenantId_WarehouseId_ProductVariantId",
                table: "WarehouseStocks",
                columns: new[] { "TenantId", "WarehouseId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStocks_WarehouseId",
                table: "WarehouseStocks",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "IdempotencyRecords");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "JournalEntryLines");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductTaxRates");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TenantSettings");

            migrationBuilder.DropTable(
                name: "UnitConversions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "WarehouseStocks");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "ChartOfAccounts");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "PaymentTerms");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasure");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
