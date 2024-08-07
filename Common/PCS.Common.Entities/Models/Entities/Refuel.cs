﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PCS.Common.Entities.Models.Entities;

[BsonIgnoreExtraElements]
public sealed record Refuel : BaseEntity
{
    public required QuickVehicle Vehicle { get; init; }
    public required MeasuredData MeasuredData { get; init; }
    public required CalculatedData CalculatedData { get; init; }

}

public sealed record QuickRefuel
{
    public required string Id { get; init; }
    public required string Station { get; init; }
    public required DateTime Date { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Cost { get; init; }
}

public sealed record MeasuredData
{
    public required string Station { get; set; }
    public required DateTime Date { get; set; }

    [BsonRepresentation(BsonType.Decimal128)] public required decimal Km { get; set; }

    [BsonRepresentation(BsonType.Decimal128)] public required decimal UnitCost { get; set; }

    [BsonRepresentation(BsonType.Decimal128)] public required decimal Liters { get; set; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Consumptions { get; set; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Autonomy { get; set; }
}

public sealed record CalculatedData
{
    [BsonRepresentation(BsonType.Decimal128)] public required decimal TotalCost { get; init; }

    [BsonRepresentation(BsonType.Decimal128)] public required decimal Consumptions { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal Autonomy { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal WeightedConsumptions { get; init; }
    [BsonRepresentation(BsonType.Decimal128)] public required decimal WeightedAutonomy { get; init; }
}
