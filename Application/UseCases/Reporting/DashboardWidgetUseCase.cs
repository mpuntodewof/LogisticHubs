using Application.DTOs.Common;
using Application.DTOs.Reporting;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Reporting
{
    public class DashboardWidgetUseCase
    {
        private readonly IDashboardWidgetRepository _repository;

        public DashboardWidgetUseCase(IDashboardWidgetRepository repository)
        {
            _repository = repository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<DashboardWidgetDto>> GetPagedAsync(PagedRequest request, Guid? userId = null)
        {
            var paged = await _repository.GetPagedAsync(request, userId);
            return new PagedResult<DashboardWidgetDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<List<DashboardWidgetDto>> GetMyWidgetsAsync(Guid? userId)
        {
            var widgets = await _repository.GetByUserIdAsync(userId);
            return widgets.Select(MapToDto).ToList();
        }

        public async Task<DashboardWidgetDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDetailDto(entity);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<DashboardWidgetDto> CreateAsync(CreateDashboardWidgetRequest request)
        {
            var entity = new DashboardWidget
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                WidgetType = request.WidgetType,
                DataSourceKey = request.DataSourceKey,
                ConfigJson = request.ConfigJson,
                PositionX = request.PositionX,
                PositionY = request.PositionY,
                Width = request.Width,
                Height = request.Height,
                SortOrder = request.SortOrder,
                IsVisible = request.IsVisible,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(entity);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateDashboardWidgetRequest request)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DashboardWidget {id} not found.");

            if (request.Title != null) entity.Title = request.Title;
            if (request.WidgetType != null) entity.WidgetType = request.WidgetType;
            if (request.DataSourceKey != null) entity.DataSourceKey = request.DataSourceKey;
            if (request.ConfigJson != null) entity.ConfigJson = request.ConfigJson;
            if (request.PositionX.HasValue) entity.PositionX = request.PositionX.Value;
            if (request.PositionY.HasValue) entity.PositionY = request.PositionY.Value;
            if (request.Width.HasValue) entity.Width = request.Width.Value;
            if (request.Height.HasValue) entity.Height = request.Height.Value;
            if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
            if (request.IsVisible.HasValue) entity.IsVisible = request.IsVisible.Value;
            if (request.UserId.HasValue) entity.UserId = request.UserId;

            await _repository.UpdateAsync(entity);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DashboardWidget {id} not found.");

            await _repository.DeleteAsync(entity);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static DashboardWidgetDto MapToDto(DashboardWidget e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            WidgetType = e.WidgetType,
            DataSourceKey = e.DataSourceKey,
            PositionX = e.PositionX,
            PositionY = e.PositionY,
            Width = e.Width,
            Height = e.Height,
            SortOrder = e.SortOrder,
            IsVisible = e.IsVisible,
            UserId = e.UserId
        };

        private static DashboardWidgetDetailDto MapToDetailDto(DashboardWidget e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            WidgetType = e.WidgetType,
            DataSourceKey = e.DataSourceKey,
            PositionX = e.PositionX,
            PositionY = e.PositionY,
            Width = e.Width,
            Height = e.Height,
            SortOrder = e.SortOrder,
            IsVisible = e.IsVisible,
            UserId = e.UserId,
            ConfigJson = e.ConfigJson
        };
    }
}
