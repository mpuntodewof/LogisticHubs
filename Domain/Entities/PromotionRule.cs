using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class PromotionRule : BaseEntity, ITenantScoped
    {
        public Guid PromotionId { get; set; }

        [Required, MaxLength(50)]
        public string RuleType { get; set; } = string.Empty;

        public int? MinQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderAmount { get; set; }

        public Guid? CustomerGroupId { get; set; }
        public Guid? CategoryId { get; set; }

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Promotion Promotion { get; set; } = null!;
        public CustomerGroup? CustomerGroup { get; set; }
        public Category? Category { get; set; }
    }
}
