using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UnitsOfMeasure
{
    public class UnitOfMeasureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UnitConversionDto
    {
        public Guid Id { get; set; }
        public Guid FromUnitId { get; set; }
        public string FromUnitAbbreviation { get; set; } = string.Empty;
        public Guid ToUnitId { get; set; }
        public string ToUnitAbbreviation { get; set; } = string.Empty;
        public decimal ConversionFactor { get; set; }
    }

    public class CreateUnitOfMeasureRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Abbreviation { get; set; } = string.Empty;
    }

    public class UpdateUnitOfMeasureRequest
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? Abbreviation { get; set; }
    }

    public class CreateUnitConversionRequest
    {
        public Guid FromUnitId { get; set; }
        public Guid ToUnitId { get; set; }
        public decimal ConversionFactor { get; set; }
    }

    public class UpdateUnitConversionRequest
    {
        public decimal ConversionFactor { get; set; }
    }
}
