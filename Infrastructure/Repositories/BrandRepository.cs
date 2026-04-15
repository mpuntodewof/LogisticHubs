using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
            => await _context.Brands.OrderBy(b => b.Name).ToListAsync();

        public async Task<PagedResult<Brand>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(b => b.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Brand?> GetByIdAsync(Guid id)
            => await _context.Brands.FindAsync(id);

        public async Task<bool> SlugExistsAsync(string slug)
            => await _context.Brands.AnyAsync(b => b.Slug == slug);

        public async Task<Brand> CreateAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            return brand;
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
        }

        public async Task DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
        }
    }
}
