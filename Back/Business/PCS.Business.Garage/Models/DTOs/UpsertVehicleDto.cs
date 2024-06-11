using PCS.Common.Entities.Models.Entities;

namespace PCS.Business.Garage.Models.DTOs;
public sealed record UpsertVehicleDto
{
    public string? Id { get; init; }
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public required string Plate { get; init; }
    public required DateTime BuyDate { get; init; }
    public required FuelType FuelType { get; init; }
    public required decimal TotalKm { get; init; }
}
