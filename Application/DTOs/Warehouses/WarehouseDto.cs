namespace Application.DTOs.Warehouses
{
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
