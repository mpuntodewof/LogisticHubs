using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "LogoUrl", "Name", "Slug", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "PT Indofood Sukses Makmur — Indonesia's largest food company", true, null, "Indofood", "indofood", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Wings Group — FMCG household and personal care", true, null, "Wings", "wings", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Unilever Indonesia — global consumer goods", true, null, "Unilever", "unilever", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "PT Santos Jaya Abadi — Indonesia's #1 coffee brand", true, null, "Kapal Api", "kapal-api", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "PT Ultra Jaya Milk — UHT milk and dairy products", true, null, "Ultra Jaya", "ultra-jaya", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "PT Mayora Indah — biscuits, confectionery, beverages", true, null, "Mayora", "mayora", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Danone-Aqua — Indonesia's leading bottled water", true, null, "Aqua", "aqua", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "Name", "ParentCategoryId", "Slug", "SortOrder", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("c0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Drinks, juices, water, and ready-to-drink beverages", true, "Beverages", null, "beverages", 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("c0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Chips, crackers, biscuits, and packaged snacks", true, "Snacks", null, "snacks", 2, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("c0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Milk, yogurt, cheese, and dairy products", true, "Dairy", null, "dairy", 3, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("c0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Rice, flour, cooking oil, and pantry essentials", true, "Rice & Staples", null, "rice-and-staples", 4, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("c0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Soap, shampoo, toothpaste, and hygiene products", true, "Personal Care", null, "personal-care", 5, new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

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

            migrationBuilder.InsertData(
                table: "TaxRates",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "EffectiveFrom", "EffectiveTo", "IsActive", "Name", "Rate", "TaxType", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("ae000000-0000-0000-0000-000000000001"), "PPN-11", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Pajak Pertambahan Nilai 11%", new DateTime(2022, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "PPN 11%", 0.1100m, "PPN", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("ae000000-0000-0000-0000-000000000002"), "TAX-EX", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Tax exempt (basic necessities)", new DateTime(2022, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Exempt", 0.0000m, "Exempt", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "UnitsOfMeasure",
                columns: new[] { "Id", "Abbreviation", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "pcs", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Piece", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "box", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Box", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), "kg", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Kilogram", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), "L", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Liter", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), "dzn", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Dozen", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "Name", "ParentCategoryId", "Slug", "SortOrder", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("c0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Ground coffee, instant coffee, and tea", true, "Coffee & Tea", new Guid("c0000000-0000-0000-0000-000000000001"), "coffee-and-tea", 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BaseUnitOfMeasureId", "BrandId", "CategoryId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "Name", "Slug", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("d0000000-0000-0000-0000-000000000001"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000001"), new Guid("c0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Indonesia's iconic instant fried noodle, original flavor", true, "Indomie Mi Goreng", "indomie-mi-goreng", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000002"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000007"), new Guid("c0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Natural mineral water from mountain springs", true, "Aqua Mineral Water", "aqua-mineral-water", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000004"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000005"), new Guid("c0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "UHT processed full cream milk, rich in calcium and vitamin D", true, "Ultra Milk UHT Full Cream", "ultra-milk-uht-full-cream", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000005"), new Guid("a0000000-0000-0000-0000-000000000003"), null, new Guid("c0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Premium fragrant pandan wangi rice, locally sourced from Central Java", true, "Beras Premium Pandan Wangi", "beras-premium-pandan-wangi", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000006"), new Guid("a0000000-0000-0000-0000-000000000004"), new Guid("b0000000-0000-0000-0000-000000000001"), new Guid("c0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Premium cooking oil, twice-filtered for clarity and health", true, "Bimoli Minyak Goreng", "bimoli-minyak-goreng", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000007"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000006"), new Guid("c0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Classic coconut-flavored cream biscuit, a household favorite", true, "Roma Kelapa Biscuit", "roma-kelapa-biscuit", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000008"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000003"), new Guid("c0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Antibacterial body wash with ActiveSilver formula", true, "Lifebuoy Body Wash", "lifebuoy-body-wash", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000009"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000002"), new Guid("c0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Concentrated liquid laundry detergent with softener", true, "So Klin Liquid Detergent", "so-klin-liquid-detergent", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000010"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000006"), new Guid("c0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Ready-to-drink jasmine green tea brewed from young tea leaves", true, "Teh Pucuk Harum", "teh-pucuk-harum", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "UnitConversions",
                columns: new[] { "Id", "ConversionFactor", "CreatedAt", "CreatedBy", "FromUnitId", "TenantId", "ToUnitId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("ac000000-0000-0000-0000-000000000001"), 12m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("a0000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("ac000000-0000-0000-0000-000000000002"), 12m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000000-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("a0000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("ac000000-0000-0000-0000-000000000003"), 1000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("a0000000-0000-0000-0000-000000000003"), null, null }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "AltText", "CreatedAt", "CreatedBy", "ImageUrl", "IsPrimary", "ProductId", "ProductVariantId", "SortOrder", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), "Indomie Mi Goreng pack", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/indomie-goreng.jpg", true, new Guid("d0000000-0000-0000-0000-000000000001"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), "Aqua mineral water bottle", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/aqua-mineral.jpg", true, new Guid("d0000000-0000-0000-0000-000000000002"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000004"), "Ultra Milk Full Cream carton", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/ultra-milk-fc.jpg", true, new Guid("d0000000-0000-0000-0000-000000000004"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000005"), "Beras Pandan Wangi sack", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/beras-pandan-wangi.jpg", true, new Guid("d0000000-0000-0000-0000-000000000005"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000006"), "Bimoli cooking oil bottle", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/bimoli-cooking-oil.jpg", true, new Guid("d0000000-0000-0000-0000-000000000006"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000007"), "Roma Kelapa biscuit pack", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/roma-kelapa.jpg", true, new Guid("d0000000-0000-0000-0000-000000000007"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000008"), "Lifebuoy body wash bottle", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/lifebuoy-body-wash.jpg", true, new Guid("d0000000-0000-0000-0000-000000000008"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000009"), "So Klin liquid detergent", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/so-klin-liquid.jpg", true, new Guid("d0000000-0000-0000-0000-000000000009"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("e1000000-0000-0000-0000-000000000010"), "Teh Pucuk Harum bottle", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/teh-pucuk-harum.jpg", true, new Guid("d0000000-0000-0000-0000-000000000010"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "ProductTaxRates",
                columns: new[] { "ProductId", "TaxRateId", "TenantId" },
                values: new object[,]
                {
                    { new Guid("d0000000-0000-0000-0000-000000000001"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000002"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000004"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000005"), new Guid("ae000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000006"), new Guid("ae000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000007"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000008"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000009"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("d0000000-0000-0000-0000-000000000010"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Barcode", "CostPrice", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "Name", "ProductId", "SellingPrice", "Sku", "TenantId", "UpdatedAt", "UpdatedBy", "VariantAttributes", "Weight" },
                values: new object[,]
                {
                    { new Guid("f0000000-0000-0000-0000-000000000001"), "8901234560001", 2200m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Indomie Mi Goreng - Single Pack", new Guid("d0000000-0000-0000-0000-000000000001"), 3000m, "IDM-GRG-001", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.085m },
                    { new Guid("f0000000-0000-0000-0000-000000000002"), "8901234560002", 82000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Indomie Mi Goreng - Box (40 pcs)", new Guid("d0000000-0000-0000-0000-000000000001"), 110000m, "IDM-GRG-040", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 3.4m },
                    { new Guid("f0000000-0000-0000-0000-000000000003"), "8901234560003", 2000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Aqua 330ml", new Guid("d0000000-0000-0000-0000-000000000002"), 3500m, "AQU-330-001", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.33m },
                    { new Guid("f0000000-0000-0000-0000-000000000004"), "8901234560004", 2800m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Aqua 600ml", new Guid("d0000000-0000-0000-0000-000000000002"), 4500m, "AQU-600-001", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.6m },
                    { new Guid("f0000000-0000-0000-0000-000000000005"), "8901234560005", 4500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Aqua 1500ml (Gallon-ette)", new Guid("d0000000-0000-0000-0000-000000000002"), 7500m, "AQU-1500-01", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 1.5m },
                    { new Guid("f0000000-0000-0000-0000-000000000008"), "8901234560008", 4500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Ultra Milk Full Cream 250ml", new Guid("d0000000-0000-0000-0000-000000000004"), 6500m, "ULT-FC-0250", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.26m },
                    { new Guid("f0000000-0000-0000-0000-000000000009"), "8901234560009", 15000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Ultra Milk Full Cream 1000ml", new Guid("d0000000-0000-0000-0000-000000000004"), 19500m, "ULT-FC-1000", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 1.03m },
                    { new Guid("f0000000-0000-0000-0000-000000000010"), "8901234560010", 62000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Beras Pandan Wangi 5kg", new Guid("d0000000-0000-0000-0000-000000000005"), 75000m, "BRS-PW-005K", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 5.0m },
                    { new Guid("f0000000-0000-0000-0000-000000000011"), "8901234560011", 290000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Beras Pandan Wangi 25kg", new Guid("d0000000-0000-0000-0000-000000000005"), 350000m, "BRS-PW-025K", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 25.0m },
                    { new Guid("f0000000-0000-0000-0000-000000000012"), "8901234560012", 17000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Bimoli Minyak Goreng 1L", new Guid("d0000000-0000-0000-0000-000000000006"), 22000m, "BMI-MG-001L", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.92m },
                    { new Guid("f0000000-0000-0000-0000-000000000013"), "8901234560013", 32000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Bimoli Minyak Goreng 2L", new Guid("d0000000-0000-0000-0000-000000000006"), 42000m, "BMI-MG-002L", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 1.84m },
                    { new Guid("f0000000-0000-0000-0000-000000000014"), "8901234560014", 8500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Roma Kelapa 300g", new Guid("d0000000-0000-0000-0000-000000000007"), 12500m, "ROM-KLP-300", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.3m },
                    { new Guid("f0000000-0000-0000-0000-000000000015"), "8901234560015", 22000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Lifebuoy Body Wash 400ml - Cool", new Guid("d0000000-0000-0000-0000-000000000008"), 32000m, "LFB-BW-400C", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.42m },
                    { new Guid("f0000000-0000-0000-0000-000000000016"), "8901234560016", 22000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Lifebuoy Body Wash 400ml - Total", new Guid("d0000000-0000-0000-0000-000000000008"), 32000m, "LFB-BW-400T", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.42m },
                    { new Guid("f0000000-0000-0000-0000-000000000017"), "8901234560017", 14000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "So Klin Liquid 800ml", new Guid("d0000000-0000-0000-0000-000000000009"), 19500m, "SKL-LQ-0800", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.82m },
                    { new Guid("f0000000-0000-0000-0000-000000000018"), "8901234560018", 24000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "So Klin Liquid 1600ml Refill", new Guid("d0000000-0000-0000-0000-000000000009"), 34000m, "SKL-LQ-1600", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 1.62m },
                    { new Guid("f0000000-0000-0000-0000-000000000019"), "8901234560019", 2500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Teh Pucuk Harum 350ml", new Guid("d0000000-0000-0000-0000-000000000010"), 4000m, "TPH-350-001", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.36m },
                    { new Guid("f0000000-0000-0000-0000-000000000020"), "8901234560020", 3200m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Teh Pucuk Harum 480ml", new Guid("d0000000-0000-0000-0000-000000000010"), 5500m, "TPH-480-001", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.49m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BaseUnitOfMeasureId", "BrandId", "CategoryId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "Name", "Slug", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("d0000000-0000-0000-0000-000000000003"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("b0000000-0000-0000-0000-000000000004"), new Guid("c0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Premium ground coffee blend, rich and aromatic", true, "Kapal Api Kopi Spesial", "kapal-api-kopi-spesial", "Active", new Guid("00000000-0000-0000-0000-000000000001"), null, null });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "AltText", "CreatedAt", "CreatedBy", "ImageUrl", "IsPrimary", "ProductId", "ProductVariantId", "SortOrder", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("e1000000-0000-0000-0000-000000000003"), "Kapal Api Kopi Spesial pack", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "/images/products/kapal-api-special.jpg", true, new Guid("d0000000-0000-0000-0000-000000000003"), null, 1, new Guid("00000000-0000-0000-0000-000000000001"), null, null });

            migrationBuilder.InsertData(
                table: "ProductTaxRates",
                columns: new[] { "ProductId", "TaxRateId", "TenantId" },
                values: new object[] { new Guid("d0000000-0000-0000-0000-000000000003"), new Guid("ae000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Barcode", "CostPrice", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "Name", "ProductId", "SellingPrice", "Sku", "TenantId", "UpdatedAt", "UpdatedBy", "VariantAttributes", "Weight" },
                values: new object[,]
                {
                    { new Guid("f0000000-0000-0000-0000-000000000006"), "8901234560006", 12000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Kapal Api Special 165g", new Guid("d0000000-0000-0000-0000-000000000003"), 17500m, "KPA-SPL-165", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.165m },
                    { new Guid("f0000000-0000-0000-0000-000000000007"), "8901234560007", 25000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, "Kapal Api Special 380g", new Guid("d0000000-0000-0000-0000-000000000003"), 35000m, "KPA-SPL-380", new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 0.38m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("e1000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000001"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000002"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000003"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000004"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000005"), new Guid("ae000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000006"), new Guid("ae000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000007"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000008"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000009"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductTaxRates",
                keyColumns: new[] { "ProductId", "TaxRateId" },
                keyValues: new object[] { new Guid("d0000000-0000-0000-0000-000000000010"), new Guid("ae000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("f0000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "UnitConversions",
                keyColumn: "Id",
                keyValue: new Guid("ac000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "UnitConversions",
                keyColumn: "Id",
                keyValue: new Guid("ac000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "UnitConversions",
                keyColumn: "Id",
                keyValue: new Guid("ac000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("ae000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "TaxRates",
                keyColumn: "Id",
                keyValue: new Guid("ae000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0001-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6049));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6064));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0003-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6069));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0004-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6083));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0005-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0006-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6093));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0007-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6098));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0008-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6177));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0009-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6183));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6190));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6195));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6200));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6204));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6213));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6218));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6222));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6246));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6251));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6256));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6260));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6264));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6269));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6280));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6296));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6301));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6305));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6309));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6363));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6401));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6406));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6411));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6415));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6421));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6425));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6429));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6434));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6454));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6458));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6463));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6467));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6471));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6507));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6512));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6516));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6521));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6525));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6528));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6533));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6537));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6542));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6547));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6551));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6555));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6562));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6566));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0058-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6570));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6574));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6579));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0061-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6583));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0062-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(6588));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5894));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5903));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5905));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 12, 6, 20, 4, 133, DateTimeKind.Utc).AddTicks(5907));
        }
    }
}
