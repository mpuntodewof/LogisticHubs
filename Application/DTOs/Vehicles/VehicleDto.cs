using Domain.Enums;

namespace Application.DTOs.Vehicles
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public decimal CapacityWeight { get; set; }
        public decimal CapacityVolume { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateVehicleRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public decimal CapacityWeight { get; set; }
        public decimal CapacityVolume { get; set; }
    }

    public class UpdateVehicleRequest
    {
        public string? VehicleType { get; set; }
        public decimal? CapacityWeight { get; set; }
        public decimal? CapacityVolume { get; set; }
        public VehicleStatus? Status { get; set; }
    }
}
