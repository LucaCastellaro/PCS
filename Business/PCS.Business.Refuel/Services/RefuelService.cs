using MongoDB.Driver.Linq;
using Entities = PCS.Common.Entities.Models.Entities;
using MongoDB.Driver;
using PCS.Business.Refuel.Models.DTOs;
using MongoDB.Bson;
using PCS.Common.Business.Extensions;
using PCS.Common.Business;

namespace PCS.Business.Refuel.Services;

public interface IRefuelService
{
    Task<ResponseDto<Entities.Refuel>> AddRefuel(AddRefuelDTO model);
    Task<int> CountRefuels(string vehicleId);
    Task<List<Entities.QuickRefuel>> GetAllQuickByVehicleAsync(string vehicleId);
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
    Task<Entities.Refuel?> GetById(string id);
    Task<bool> DeleteById(string id);
}

public sealed class RefuelService(IRepository<Entities.Refuel> repo) : IRefuelService
{
    public async Task<ResponseDto<Entities.Refuel>> AddRefuel(AddRefuelDTO model)
    {
        var calculatedAutonomy = model.Liters * model.Consumptions;
        var calculatedconsumptions = model.Autonomy / model.Liters;

        var objectId = ObjectId.GenerateNewId().ToString();

        var toAdd = new Entities.Refuel()
        {
            Id = objectId,
            Vehicle = model.Vehicle,
            MeasuredData = new()
            {
                Autonomy = model.Autonomy.Round(3),
                Consumptions = model.Consumptions.Round(3),
                Date = model.Date,
                Km = model.Km.Round(3),
                Liters = model.Liters.Round(3),
                Station = model.Station.Sanitize(),
                UnitCost = model.UnitCost.Round(3),
            },
            CalculatedData = new()
            {
                Autonomy = calculatedAutonomy.Round(3),
                Consumptions = calculatedconsumptions.Round(3),
                TotalCost = (model.UnitCost * model.Liters).Round(3),
                WeightedConsumptions = GetWeightedConsumptions(calculatedconsumptions, calculatedAutonomy).Round(3),
                WeightedAutonomy = GetWeightedAutonomy(model.Liters, calculatedAutonomy).Round(3)
            }
        };

        await repo.AddAsync(toAdd);

        var result = new ResponseDto<Entities.Refuel>
        {
            Data = await repo.FindByIdAsync(objectId)
        };

        if (result.Data is null) {
            result.Errors.Add($"Impossibile aggiungere il rifornimento al veicolo targato {model.Vehicle.Plate}");
        }

        return result;
    }

    public async Task<Entities.Refuel?> GetById(string id)
        => await repo.FindByIdAsync(id);

    public async Task<bool> DeleteById(string id)
        => await repo.DeleteAsync(id);

    public IMongoQueryable<Entities.Refuel> GetAllByVehicle(string vehicleId)
        => repo.FindAllAsQueryable(xx => xx.Vehicle.Id == vehicleId);

    public async Task<List<Entities.Refuel>> GetAllByVehicleAsync(string vehicleId)
        => await repo.FindAllAsync(xx => xx.Vehicle.Id == vehicleId);

    public async Task<List<Entities.QuickRefuel>> GetAllQuickByVehicleAsync(string vehicleId)
        => await GetAllByVehicle(vehicleId)
            .Select(xx => new Entities.QuickRefuel
            {
                Id = xx.Id,
                Cost = xx.CalculatedData.TotalCost,
                Date = xx.MeasuredData.Date,
                Station = xx.MeasuredData.Station
            })
            .ToListAsync();

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
        => (await GetAllByVehicleAsync(vehicleId)).Sum(xx => xx.CalculatedData.WeightedConsumptions);

    public async Task<decimal> SumWeightedAutonomy(string vehicleId)
        => (await GetAllByVehicleAsync(vehicleId)).Sum(xx => xx.CalculatedData.WeightedAutonomy);

    private decimal GetWeightedAutonomy(decimal liters, decimal autonomy) => liters * autonomy;

    private decimal GetWeightedConsumptions(decimal consumptions, decimal autonomy) => consumptions * autonomy;
}
