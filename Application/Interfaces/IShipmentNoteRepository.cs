using Domain.Entities;

namespace Application.Interfaces
{
    public interface IShipmentNoteRepository
    {
        Task<IEnumerable<ShipmentNote>> GetByShipmentIdAsync(Guid shipmentId);
        Task<ShipmentNote?> GetByIdAsync(Guid id);
        Task<ShipmentNote> CreateAsync(ShipmentNote note);
        Task DeleteAsync(ShipmentNote note);
    }
}
