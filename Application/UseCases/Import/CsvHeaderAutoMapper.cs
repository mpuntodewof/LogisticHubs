using Application.DTOs.Import;

namespace Application.UseCases.Import
{
    public static class CsvHeaderAutoMapper
    {
        // Substring synonyms per target field. All comparisons are case-insensitive.
        // Order within an array doesn't matter; order *across* fields below DOES — earlier
        // fields claim a header first when multiple fields would match.
        // Bahasa + English synonyms cover Tokopedia, Shopee, Lazada, TikTok Shop, Bukalapak.

        private static readonly string[] OrderNumberKeywords =
            { "order id", "order no", "order number", "no pesanan", "nomor pesanan", "no. pesanan", "invoice", "no invoice" };

        private static readonly string[] SkuKeywords =
            { "sku", "kode produk", "product code", "kode sku" };

        private static readonly string[] QuantityKeywords =
            { "quantity", "qty", "jumlah", "kuantitas" };

        // "total" must be checked BEFORE generic "price"/"harga" because "Total Price"
        // / "Total Harga" / "Subtotal" would otherwise land in unit price.
        private static readonly string[] TotalPriceKeywords =
            { "total price", "total harga", "subtotal", "grand total", "total amount", "total pembayaran", "total" };

        private static readonly string[] UnitPriceKeywords =
            { "unit price", "harga satuan", "harga jual", "selling price", "price", "harga" };

        private static readonly string[] ProductNameKeywords =
            { "product name", "nama produk", "nama barang", "item name", "produk" };

        private static readonly string[] OrderDateKeywords =
            { "order date", "tanggal pesanan", "tanggal pembayaran", "tanggal transaksi", "tanggal", "date" };

        private static readonly string[] PlatformFeeKeywords =
            { "platform fee", "biaya platform", "admin fee", "biaya admin", "service fee", "fee", "biaya" };

        public static CsvColumnMapping Map(IReadOnlyList<string> headers)
        {
            ArgumentNullException.ThrowIfNull(headers);
            var mapping = new CsvColumnMapping();
            var claimed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Field order matters: each pass picks the *best* still-unclaimed header for that field.
            mapping.OrderNumberColumn = ClaimBest(headers, OrderNumberKeywords, claimed) ?? string.Empty;
            mapping.SkuColumn = ClaimBest(headers, SkuKeywords, claimed) ?? string.Empty;
            mapping.QuantityColumn = ClaimBest(headers, QuantityKeywords, claimed) ?? string.Empty;
            mapping.TotalPriceColumn = ClaimBest(headers, TotalPriceKeywords, claimed);
            mapping.UnitPriceColumn = ClaimBest(headers, UnitPriceKeywords, claimed) ?? string.Empty;
            mapping.ProductNameColumn = ClaimBest(headers, ProductNameKeywords, claimed);
            mapping.OrderDateColumn = ClaimBest(headers, OrderDateKeywords, claimed);
            mapping.PlatformFeeColumn = ClaimBest(headers, PlatformFeeKeywords, claimed);

            return mapping;
        }

        // Returns the header whose lowercased text contains the longest keyword match.
        // Longest-match-wins prevents "total" from stealing "total price" away from the
        // total-price field on a header set that has both columns.
        private static string? ClaimBest(
            IReadOnlyList<string> headers, string[] keywords, HashSet<string> claimed)
        {
            string? bestHeader = null;
            int bestKeywordLength = 0;

            foreach (var header in headers)
            {
                if (string.IsNullOrWhiteSpace(header) || claimed.Contains(header)) continue;
                // Normalize underscores/dashes to spaces so "no_pesanan", "order-number",
                // and "Order Number" all match the same keyword set.
                var normalized = header.ToLowerInvariant().Replace('_', ' ').Replace('-', ' ');

                foreach (var kw in keywords)
                {
                    if (normalized.Contains(kw) && kw.Length > bestKeywordLength)
                    {
                        bestHeader = header;
                        bestKeywordLength = kw.Length;
                    }
                }
            }

            if (bestHeader != null) claimed.Add(bestHeader);
            return bestHeader;
        }
    }
}
