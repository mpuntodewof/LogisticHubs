using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCsvImportEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlatformFeePercent = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesChannels_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CsvImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalesChannelId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WarehouseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    SuccessRows = table.Column<int>(type: "int", nullable: false),
                    FailedRows = table.Column<int>(type: "int", nullable: false),
                    SkippedRows = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ErrorSummary = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsvImportBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CsvImportBatches_SalesChannels_SalesChannelId",
                        column: x => x.SalesChannelId,
                        principalTable: "SalesChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsvImportBatches_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsvImportBatches_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CsvImportRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CsvImportBatchId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sku = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MatchedProductVariantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    StockMovementId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ErrorMessage = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RawRowJson = table.Column<string>(type: "text", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsvImportRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CsvImportRows_CsvImportBatches_CsvImportBatchId",
                        column: x => x.CsvImportBatchId,
                        principalTable: "CsvImportBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CsvImportRows_ProductVariants_MatchedProductVariantId",
                        column: x => x.MatchedProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CsvImportRows_StockMovements_StockMovementId",
                        column: x => x.StockMovementId,
                        principalTable: "StockMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CsvImportRows_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0001-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3839));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3853));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0003-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3857));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0004-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3915));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0005-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3920));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0006-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3924));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0007-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3928));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0008-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4049));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0009-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4054));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4059));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4063));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4066));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4070));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4073));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4077));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4081));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4084));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4102));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4107));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4110));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4114));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4117));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4120));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4129));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4144));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4149));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4152));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4155));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4265));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4302));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4306));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4309));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4313));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4317));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4321));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4324));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4328));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4341));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4344));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4348));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4351));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4354));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4357));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4360));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4401));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4405));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4408));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4412));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4415));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4418));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4422));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4426));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4429));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4433));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4436));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4440));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4443));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0058-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4446));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4450));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4453));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0061-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4456));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0062-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(4460));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3698));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3703));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3705));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3707));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 14, 7, 55, 33, 434, DateTimeKind.Utc).AddTicks(3709));

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CompanyName",
                value: "StockLedger Default");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "Email",
                value: "admin@stockledger.io");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "Email",
                value: "manager@stockledger.io");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "Email",
                value: "viewer@stockledger.io");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "Email",
                value: "warehouse@stockledger.io");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "Email",
                value: "accountant@stockledger.io");

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportBatches_SalesChannelId",
                table: "CsvImportBatches",
                column: "SalesChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportBatches_TenantId",
                table: "CsvImportBatches",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportBatches_WarehouseId",
                table: "CsvImportBatches",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportRows_CsvImportBatchId_RowNumber",
                table: "CsvImportRows",
                columns: new[] { "CsvImportBatchId", "RowNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportRows_MatchedProductVariantId",
                table: "CsvImportRows",
                column: "MatchedProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportRows_StockMovementId",
                table: "CsvImportRows",
                column: "StockMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_CsvImportRows_TenantId",
                table: "CsvImportRows",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesChannels_TenantId_Slug",
                table: "SalesChannels",
                columns: new[] { "TenantId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CsvImportRows");

            migrationBuilder.DropTable(
                name: "CsvImportBatches");

            migrationBuilder.DropTable(
                name: "SalesChannels");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0001-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3641));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3654));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0003-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3659));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0004-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3669));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0005-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3673));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0006-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3678));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0007-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3682));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0008-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3691));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0009-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3695));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3700));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3705));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3709));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3713));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3717));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3721));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3725));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3769));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3787));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3792));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3796));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3799));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3803));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3807));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3815));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3828));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3832));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3836));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3840));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3844));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3887));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3891));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3895));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3899));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3905));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3909));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3912));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3916));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3968));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3972));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3976));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3980));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3984));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3988));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3991));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3995));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3999));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4003));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4007));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4010));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4014));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4018));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4022));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4026));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4030));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4058));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4063));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4068));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0058-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4071));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4075));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4079));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0061-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4083));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0062-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(4087));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3455));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3459));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3461));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3463));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 7, 31, 38, 29, DateTimeKind.Utc).AddTicks(3465));

            migrationBuilder.UpdateData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CompanyName",
                value: "NiagaOne Default");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "Email",
                value: "admin@niagaone.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "Email",
                value: "manager@niagaone.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "Email",
                value: "viewer@niagaone.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "Email",
                value: "warehouse@niagaone.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "Email",
                value: "accountant@niagaone.com");
        }
    }
}
