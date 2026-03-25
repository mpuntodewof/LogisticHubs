using Domain.Enums;

namespace Application.DTOs.Drivers
{
    public class DriverDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CreateDriverRequest
    {
        public Guid UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class UpdateDriverRequest
    {
        public string? LicenseNumber { get; set; }
        public string? Phone { get; set; }
        public DriverStatus? Status { get; set; }
    }
}
