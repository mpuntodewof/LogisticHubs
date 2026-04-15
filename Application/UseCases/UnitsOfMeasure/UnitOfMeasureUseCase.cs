using Application.DTOs.Common;
using Application.DTOs.UnitsOfMeasure;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.UnitsOfMeasure
{
    public class UnitOfMeasureUseCase
    {
        private readonly IUnitOfMeasureRepository _unitRepository;
        private readonly IUnitConversionRepository _conversionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfMeasureUseCase(
            IUnitOfMeasureRepository unitRepository,
            IUnitConversionRepository conversionRepository,
            IUnitOfWork unitOfWork)
        {
            _unitRepository = unitRepository;
            _conversionRepository = conversionRepository;
            _unitOfWork = unitOfWork;
        }

        // ── UOM Get ──────────────────────────────────────────────────────────────

        public async Task<PagedResult<UnitOfMeasureDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _unitRepository.GetPagedAsync(request);
            return new PagedResult<UnitOfMeasureDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<UnitOfMeasureDto?> GetByIdAsync(Guid id)
        {
            var unit = await _unitRepository.GetByIdAsync(id);
            return unit == null ? null : MapToDto(unit);
        }

        // ── UOM Create ───────────────────────────────────────────────────────────

        public async Task<UnitOfMeasureDto> CreateAsync(CreateUnitOfMeasureRequest request)
        {
            if (await _unitRepository.AbbreviationExistsAsync(request.Abbreviation))
                throw new InvalidOperationException($"A unit with abbreviation '{request.Abbreviation}' already exists.");

            var unit = new Domain.Entities.UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Abbreviation = request.Abbreviation,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _unitRepository.CreateAsync(unit);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(created);
        }

        // ── UOM Update ───────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateUnitOfMeasureRequest request)
        {
            var unit = await _unitRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Unit of measure {id} not found.");

            if (request.Name != null) unit.Name = request.Name;
            if (request.Abbreviation != null) unit.Abbreviation = request.Abbreviation;

            await _unitRepository.UpdateAsync(unit);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── UOM Delete ───────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var unit = await _unitRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Unit of measure {id} not found.");

            if (await _unitRepository.IsInUseByProductsAsync(id))
                throw new InvalidOperationException("Cannot delete a unit of measure that is in use by products.");

            await _unitRepository.DeleteAsync(unit);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Conversions Get ──────────────────────────────────────────────────────

        public async Task<IEnumerable<UnitConversionDto>> GetConversionsAsync(Guid unitId)
        {
            var conversions = await _conversionRepository.GetByFromUnitAsync(unitId);
            return conversions.Select(MapConversionToDto);
        }

        // ── Conversions Create ───────────────────────────────────────────────────

        public async Task<UnitConversionDto> CreateConversionAsync(CreateUnitConversionRequest request)
        {
            _ = await _unitRepository.GetByIdAsync(request.FromUnitId)
                ?? throw new KeyNotFoundException($"From unit {request.FromUnitId} not found.");

            _ = await _unitRepository.GetByIdAsync(request.ToUnitId)
                ?? throw new KeyNotFoundException($"To unit {request.ToUnitId} not found.");

            if (request.FromUnitId == request.ToUnitId)
                throw new InvalidOperationException("Cannot create a conversion from a unit to itself.");

            if (await _conversionRepository.ExistsAsync(request.FromUnitId, request.ToUnitId))
                throw new InvalidOperationException("A conversion between these units already exists.");

            var conversion = new UnitConversion
            {
                Id = Guid.NewGuid(),
                FromUnitId = request.FromUnitId,
                ToUnitId = request.ToUnitId,
                ConversionFactor = request.ConversionFactor,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _conversionRepository.CreateAsync(conversion);
            await _unitOfWork.SaveChangesAsync();
            return MapConversionToDto(created);
        }

        // ── Conversions Update ───────────────────────────────────────────────────

        public async Task UpdateConversionAsync(Guid id, UpdateUnitConversionRequest request)
        {
            var conversion = await _conversionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Unit conversion {id} not found.");

            conversion.ConversionFactor = request.ConversionFactor;

            await _conversionRepository.UpdateAsync(conversion);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Conversions Delete ───────────────────────────────────────────────────

        public async Task DeleteConversionAsync(Guid id)
        {
            var conversion = await _conversionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Unit conversion {id} not found.");

            await _conversionRepository.DeleteAsync(conversion);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static UnitOfMeasureDto MapToDto(Domain.Entities.UnitOfMeasure u) => new()
        {
            Id = u.Id,
            Name = u.Name,
            Abbreviation = u.Abbreviation,
            CreatedAt = u.CreatedAt
        };

        private static UnitConversionDto MapConversionToDto(UnitConversion c) => new()
        {
            Id = c.Id,
            FromUnitId = c.FromUnitId,
            FromUnitAbbreviation = c.FromUnit?.Abbreviation ?? string.Empty,
            ToUnitId = c.ToUnitId,
            ToUnitAbbreviation = c.ToUnit?.Abbreviation ?? string.Empty,
            ConversionFactor = c.ConversionFactor
        };
    }
}
