namespace PCS.Business.Refuel.Models.DTOs;
public sealed record EditRefuelDTO
{
    public required string Id { get; init; }
    public required string Station { get; init; }
    public required DateTime Date { get; init; }
    public required decimal Km { get; init; }
    public required decimal Consumptions { get; init; }
    public required decimal UnitCost { get; init; }
    public required decimal Liters { get; init; }
    public required decimal Autonomy { get; init; }
}
