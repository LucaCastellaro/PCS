using Entities = PCS.Common.Entities.Models.Entities;

namespace PCS.Business.Refuel.Extensions;
public static class RefuelExtensions
{
    public static decimal GetWeightedConsumptions(this Entities.Refuel source) => source.CalculatedData.Consumptions * source.CalculatedData.Autonomy;
    public static decimal GetWeightedAutonomy(this Entities.Refuel source) => source.CalculatedData.Autonomy * source.CalculatedData.Liters;
}