using PCS.Common.Entities.Models.Entities;

namespace PCS.Common.Entities.Models.Dtos;
public sealed record FuelTypeDto
{
    public required string Label { get; init; }
    public required FuelType Value { get; init; }
}
