namespace BlazorApp.Client.Models
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
        public string? Status { get; set; }
    }

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
        public string? Status { get; set; }
    }

    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateWarehouseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    public class UpdateWarehouseRequest
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
    }
}
