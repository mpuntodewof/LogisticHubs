using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Loyalty
{
    public class LoyaltyMembershipDto
    {
        public Guid Id { get; set; }
        public Guid LoyaltyProgramId { get; set; }
        public string? ProgramName { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? CurrentTierId { get; set; }
        public string? CurrentTierName { get; set; }
        public int AvailablePoints { get; set; }
        public int LifetimePoints { get; set; }
        public int TotalRedeemed { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class EnrollCustomerRequest
    {
        [Required]
        public Guid LoyaltyProgramId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
    }
}
