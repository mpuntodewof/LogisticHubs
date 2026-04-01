using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly AppDbContext _context;

        public NotificationTemplateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<NotificationTemplate>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.NotificationTemplates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(t => t.Code.ToLower().Contains(search) || t.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "code" => request.SortDescending ? query.OrderByDescending(t => t.Code) : query.OrderBy(t => t.Code),
                "name" => request.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                _ => query.OrderBy(t => t.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<NotificationTemplate?> GetByIdAsync(Guid id)
            => await _context.NotificationTemplates.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<NotificationTemplate?> GetByCodeAsync(string code)
            => await _context.NotificationTemplates.FirstOrDefaultAsync(t => t.Code == code);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.NotificationTemplates.AnyAsync(t => t.Code == code);

        public async Task<NotificationTemplate> CreateAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task UpdateAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }
}
