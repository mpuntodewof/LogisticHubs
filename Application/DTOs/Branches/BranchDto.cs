using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Branches
{
    public class BranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BranchUserDto
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateBranchRequest
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Province { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        public Guid? WarehouseId { get; set; }
    }

    public class UpdateBranchRequest
    {
        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? City { get; set; }

        [StringLength(255)]
        public string? Province { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        public Guid? WarehouseId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AssignBranchUserRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
