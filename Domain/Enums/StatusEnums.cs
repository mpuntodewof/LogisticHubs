namespace Domain.Enums
{
    public enum UserRole
    {
        Admin,
        Manager,
        Driver,
        Viewer
    }

    public enum DriverStatus
    {
        Available,
        OnDuty,
        OffDuty,
        Suspended
    }

    public enum VehicleStatus
    {
        Available,
        InUse,
        UnderMaintenance,
        Retired
    }

    public enum ShipmentStatus
    {
        Pending,
        Assigned,
        PickedUp,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed,
        Cancelled
    }
}
