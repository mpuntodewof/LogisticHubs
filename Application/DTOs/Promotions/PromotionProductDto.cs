namespace Application.DTOs.Promotions
{
    public class PromotionProductDto
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string? VariantName { get; set; }
        public bool IsGetItem { get; set; }
    }

    public class CreatePromotionProductRequest
    {
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public bool IsGetItem { get; set; }
    }
}
