using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedFinanceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent seed: INSERT IGNORE so re-running on a DB that already
            // has these rows (e.g. from an earlier ad-hoc seed or partial apply)
            // is a no-op instead of a duplicate-key error. Avoids the FK trap of
            // DELETE-then-INSERT when JournalEntryLines already reference the
            // chart of accounts.
            migrationBuilder.Sql(@"
INSERT IGNORE INTO `ChartOfAccounts`
    (`Id`, `AccountCode`, `AccountSubType`, `AccountType`, `CreatedAt`, `CreatedBy`, `DeletedAt`, `DeletedBy`, `Description`, `IsActive`, `Name`, `NormalBalance`, `ParentAccountId`, `TenantId`, `UpdatedAt`, `UpdatedBy`)
VALUES
    ('f1000000-0000-0000-0000-000000000001', '1000', NULL, 'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 'Aset',                  'Debit',  NULL, '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000000002', '2000', NULL, 'Liability', '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 'Liabilitas',            'Credit', NULL, '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000000003', '3000', NULL, 'Equity',    '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 'Ekuitas',               'Credit', NULL, '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000000004', '4000', NULL, 'Revenue',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 'Pendapatan',            'Credit', NULL, '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000000005', '5000', NULL, 'Expense',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 'Harga Pokok Penjualan', 'Debit',  NULL, '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000000006', '6000', NULL, 'Expense',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 'Beban Operasional',     'Debit',  NULL, '00000000-0000-0000-0000-000000000001', NULL, NULL);
");

            migrationBuilder.Sql(@"
INSERT IGNORE INTO `PaymentTerms`
    (`Id`, `Code`, `CreatedAt`, `CreatedBy`, `DeletedAt`, `DeletedBy`, `Description`, `DueDays`, `IsActive`, `Name`, `TenantId`, `UpdatedAt`, `UpdatedBy`)
VALUES
    ('f2000000-0000-0000-0000-000000000001', 'COD',   '2026-01-01 00:00:00', NULL, NULL, NULL, 'Bayar saat barang diterima',  0, 1, 'Cash on Delivery', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f2000000-0000-0000-0000-000000000002', 'NET7',  '2026-01-01 00:00:00', NULL, NULL, NULL, 'Pembayaran dalam 7 hari',     7, 1, 'Net 7',            '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f2000000-0000-0000-0000-000000000003', 'NET14', '2026-01-01 00:00:00', NULL, NULL, NULL, 'Pembayaran dalam 14 hari',   14, 1, 'Net 14',           '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f2000000-0000-0000-0000-000000000004', 'NET30', '2026-01-01 00:00:00', NULL, NULL, NULL, 'Pembayaran dalam 30 hari',   30, 1, 'Net 30',           '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f2000000-0000-0000-0000-000000000005', 'NET60', '2026-01-01 00:00:00', NULL, NULL, NULL, 'Pembayaran dalam 60 hari',   60, 1, 'Net 60',           '00000000-0000-0000-0000-000000000001', NULL, NULL);
");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0001-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(752));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0002-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(768));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0003-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(773));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0004-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(786));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0005-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(791));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0006-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(796));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0007-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(801));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0008-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(857));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0009-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(863));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0010-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(868));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0011-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(873));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0012-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(877));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0013-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(881));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0014-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(885));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0015-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(889));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0016-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(894));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0017-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(898));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0018-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(915));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0019-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(919));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0020-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(923));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0021-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(927));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0022-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(932));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0023-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(936));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0024-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(941));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0025-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(953));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0026-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(958));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0027-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(962));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0028-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1054));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0029-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1059));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0030-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1089));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0031-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1094));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0032-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1098));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0033-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1101));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0034-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1106));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0035-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1110));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0036-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1113));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0037-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1117));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0038-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1132));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0039-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1136));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0040-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1141));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0041-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1144));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0042-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1148));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0043-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1220));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0044-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1225));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0045-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0046-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1233));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0047-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1237));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0048-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1241));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0049-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1245));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0050-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1249));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0051-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1253));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0052-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1258));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0053-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1262));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0054-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1266));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0055-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1270));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0056-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1274));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0057-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1279));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0058-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1282));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0059-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1286));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0060-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1291));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0061-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1295));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0062-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(1299));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(590));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(599));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(601));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(603));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 16, 8, 34, 28, 923, DateTimeKind.Utc).AddTicks(605));

            // Same idempotent INSERT IGNORE pattern for the detailed account
            // hierarchy. Two column sets here: the IsSystemAccount column is
            // present on a subset of these inserts; we emit it explicitly per row.
            migrationBuilder.Sql(@"
INSERT IGNORE INTO `ChartOfAccounts`
    (`Id`, `AccountCode`, `AccountSubType`, `AccountType`, `CreatedAt`, `CreatedBy`, `DeletedAt`, `DeletedBy`, `Description`, `IsActive`, `IsSystemAccount`, `Name`, `NormalBalance`, `ParentAccountId`, `TenantId`, `UpdatedAt`, `UpdatedBy`)
VALUES
    ('f1000000-0000-0000-0000-000000001100', '1100', 'Cash',             'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Kas & Bank',                 'Debit',  'f1000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000001200', '1200', 'Receivable',       'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 1, 'Piutang Usaha (AR)',         'Debit',  'f1000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000001300', '1300', 'Inventory',        'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 1, 'Persediaan Barang',          'Debit',  'f1000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000002100', '2100', 'Payable',          'Liability', '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 1, 'Hutang Usaha (AP)',          'Credit', 'f1000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000002200', '2200', 'Tax',              'Liability', '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 1, 'PPN Keluaran',               'Credit', 'f1000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000002210', '2210', 'Tax',              'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 1, 'PPN Masukan',                'Debit',  'f1000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000003100', '3100', 'Capital',          'Equity',    '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Modal Pemilik',              'Credit', 'f1000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000003200', '3200', 'RetainedEarnings', 'Equity',    '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Laba Ditahan',               'Credit', 'f1000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000004100', '4100', 'Sales',            'Revenue',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Penjualan - Tokopedia',      'Credit', 'f1000000-0000-0000-0000-000000000004', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000004200', '4200', 'Sales',            'Revenue',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Penjualan - Shopee',         'Credit', 'f1000000-0000-0000-0000-000000000004', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000004300', '4300', 'Sales',            'Revenue',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Penjualan - Offline',        'Credit', 'f1000000-0000-0000-0000-000000000004', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000005100', '5100', 'COGS',             'Expense',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 1, 'HPP - Barang Dagang',        'Debit',  'f1000000-0000-0000-0000-000000000005', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000006100', '6100', 'PlatformFee',      'Expense',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Biaya Platform - Tokopedia', 'Debit',  'f1000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000006200', '6200', 'PlatformFee',      'Expense',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Biaya Platform - Shopee',    'Debit',  'f1000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000006300', '6300', 'Shipping',         'Expense',   '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Biaya Pengiriman',           'Debit',  'f1000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000001110', '1110', 'Cash',             'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Kas Tunai',                  'Debit',  'f1000000-0000-0000-0000-000000001100', '00000000-0000-0000-0000-000000000001', NULL, NULL),
    ('f1000000-0000-0000-0000-000000001120', '1120', 'Bank',             'Asset',     '2026-01-01 00:00:00', NULL, NULL, NULL, NULL, 1, 0, 'Bank BCA',                   'Debit',  'f1000000-0000-0000-0000-000000001100', '00000000-0000-0000-0000-000000000001', NULL, NULL);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000001110"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000001120"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000001200"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000001300"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000002100"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000002200"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000002210"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000003100"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000003200"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000004100"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000004200"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000004300"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000005100"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000006100"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000006200"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000006300"));

            migrationBuilder.DeleteData(
                table: "PaymentTerms",
                keyColumn: "Id",
                keyValue: new Guid("f2000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "PaymentTerms",
                keyColumn: "Id",
                keyValue: new Guid("f2000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "PaymentTerms",
                keyColumn: "Id",
                keyValue: new Guid("f2000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "PaymentTerms",
                keyColumn: "Id",
                keyValue: new Guid("f2000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "PaymentTerms",
                keyColumn: "Id",
                keyValue: new Guid("f2000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000001100"));

            migrationBuilder.DeleteData(
                table: "ChartOfAccounts",
                keyColumn: "Id",
                keyValue: new Guid("f1000000-0000-0000-0000-000000000001"));

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
        }
    }
}
