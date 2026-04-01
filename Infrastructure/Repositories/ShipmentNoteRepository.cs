using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ShipmentNoteRepository : IShipmentNoteRepository
    {
        private readonly AppDbContext _context;

        public ShipmentNoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShipmentNote>> GetByShipmentIdAsync(Guid shipmentId)
            => await _context.ShipmentNotes
                .Where(n => n.ShipmentId == shipmentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<ShipmentNote?> GetByIdAsync(Guid id)
            => await _context.ShipmentNotes.FindAsync(id);

        public async Task<ShipmentNote> CreateAsync(ShipmentNote note)
        {
            _context.ShipmentNotes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task DeleteAsync(ShipmentNote note)
        {
            _context.ShipmentNotes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}
