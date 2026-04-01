using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly AppDbContext _context;

        public ApiKeyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ApiKey>> GetPagedAsync(PagedRequest request, bool? isActive = null)
        {
            var query = _context.ApiKeys.AsQueryable();

            if (isActive.HasValue)
                query = query.Where(k => k.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(k => k.Name.ToLower().Contains(search) || k.KeyPrefix.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(k => k.Name) : query.OrderBy(k => k.Name),
                _ => query.OrderByDescending(k => k.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<ApiKey?> GetByIdAsync(Guid id)
            => await _context.ApiKeys.FirstOrDefaultAsync(k => k.Id == id);

        public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
            => await _context.ApiKeys.IgnoreQueryFilters().FirstOrDefaultAsync(k => k.KeyHash == keyHash);

        public async Task<bool> NameExistsAsync(string name)
            => await _context.ApiKeys.AnyAsync(k => k.Name == name);

        public async Task<ApiKey> CreateAsync(ApiKey apiKey)
        {
            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();
            return apiKey;
        }

        public async Task UpdateAsync(ApiKey apiKey)
        {
            _context.ApiKeys.Update(apiKey);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ApiKey apiKey)
        {
            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
        }
    }
}
