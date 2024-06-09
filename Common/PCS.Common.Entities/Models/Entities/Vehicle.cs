using CNAS.Repository.Models.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PCS.Common.Entities.Models.Entities;

[BsonIgnoreExtraElements]
public sealed record Vehicle : BaseEntity
{
    public required string Name { get; init; }
    public required string Plate { get; init; }
    [BsonRepresentation(BsonType.String)] public required FuelType FuelType { get; init; }
    public required string Photo { get; init; }
    public required string BuyDate { get; init; }
    public required string InsertDate { get; init; }
    public required string TotalKm { get; init; }
    public required string TotalFuel { get; init; }
    public required QuickMemorandum[] Memorandums { get; init; }
    public required AverageValues CurrentAverageValues { get; init; }
    public required AverageValues PreviousAverageValues { get; init; }
}

public sealed record QuickVehicle
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Plate { get; init; }
    public required string Photo { get; init; }
}

public enum FuelType
{
    Gasoline,
    Diesel,
    Electric,
    Hybryd_Gasoline_Electric,
}

public sealed record QuickMemorandum
{
    public required string Name { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Value { get; init; }
    public required DateTime DueDate { get; init; }
    public required string Details { get; init; }
}

public sealed record AverageValues
{
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Consumptions { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Expanses { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal FuelCost { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Autonomy { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Liters { get; init; }
}