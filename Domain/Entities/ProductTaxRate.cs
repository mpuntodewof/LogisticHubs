using Domain.Interfaces;

namespace Domain.Entities
{
    public class ProductTaxRate : ITenantScoped
    {
        public Guid ProductId { get; set; }
        public Guid TaxRateId { get; set; }
        public Guid TenantId { get; set; }

        // Navigation
        public Product Product { get; set; } = null!;
        public TaxRate TaxRate { get; set; } = null!;
    }
}
