using MongoDB.Driver.Linq;
using PCS.Business.Refuel.Extensions;
using Entities = PCS.Common.Entities.Models.Entities;
using SpRepo.Abstraction;

namespace PCS.Business.Refuel.Services;

public interface IRefuelService
{
    Task<int> CountRefuels(string vehicleId);
    IMongoQueryable<Entities.Refuel> GetAllByVehicle(string vehicleId);
    Task<List<Entities.Refuel>> GetAllByVehicleAsync(string vehicleId);
    Task<decimal> GetAverageAutonomy(string vehicleId);
    Task<decimal> GetAverageConsumptions(string vehicleId);
    Task<decimal> GetAverageFuelUnitCost(string vehicleId);
    Task<decimal> SumCalculatedAutonomy(string vehicleId);
    Task<decimal> SumLiters(string vehicleId);
    Task<decimal> SumMeasuredAutonomy(string vehicleId);
    Task<decimal> SumTotalCosts(string vehicleId);
    Task<decimal> SumWeightedAutonomy(string vehicleId);
    Task<decimal> SumWeightedConsumptions(string vehicleId);
}

public class RefuelService(IRepository<Entities.Refuel> repo) : IRefuelService
{
    public IMongoQueryable<Entities.Refuel> GetAllByVehicle(string vehicleId)
        => repo.FindAllAsQueryable(xx => xx.Vehicle.Id == vehicleId);

    public async Task<List<Entities.Refuel>> GetAllByVehicleAsync(string vehicleId)
        => await repo.FindAllAsync(xx => xx.Vehicle.Id == vehicleId);

    public async Task<int> CountRefuels(string vehicleId)
        => (int)await repo.CountAsync(xx => xx.Vehicle.Id == vehicleId);

    public async Task<decimal> GetAverageAutonomy(string vehicleId)
        => await SumWeightedAutonomy(vehicleId) / await SumLiters(vehicleId);

    public async Task<decimal> GetAverageConsumptions(string vehicleId)
        => await SumWeightedConsumptions(vehicleId) / await SumMeasuredAutonomy(vehicleId);

    public async Task<decimal> GetAverageFuelUnitCost(string vehicleId)
        => await SumTotalCosts(vehicleId) / await SumLiters(vehicleId);

    public async Task<decimal> SumCalculatedAutonomy(string vehicleId)
        => await GetAllByVehicle(vehicleId).SumAsync(xx => xx.CalculatedData.Autonomy);

    public async Task<decimal> SumLiters(string vehicleId)
        => await GetAllByVehicle(vehicleId).SumAsync(xx => xx.MeasuredData.Liters);

    public async Task<decimal> SumMeasuredAutonomy(string vehicleId)
        => await GetAllByVehicle(vehicleId).SumAsync(xx => xx.MeasuredData.Autonomy);

    public async Task<decimal> SumTotalCosts(string vehicleId)
        => await GetAllByVehicle(vehicleId).SumAsync(xx => xx.CalculatedData.TotalCost);

    public async Task<decimal> SumWeightedConsumptions(string vehicleId)
        => (await GetAllByVehicleAsync(vehicleId)).Sum(xx => xx.GetWeightedConsumptions());

    public async Task<decimal> SumWeightedAutonomy(string vehicleId)
        => (await GetAllByVehicleAsync(vehicleId)).Sum(xx => xx.GetWeightedAutonomy());
}
