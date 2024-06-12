using PCS.Common.Entities.Models.Entities;
using SpRepo.Abstraction;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using PCS.Common.Entities.Models.Dtos;
using PCS.Business.Garage.Models.DTOs;
using PCS.Common.Business.Extensions;
using Serilog;

namespace PCS.Business.Garage.Services;

public interface IVehicleService
{
    Task<ResponseDto<List<QuickVehicle>>> GetAllVehicles(string userId, CancellationToken ct = default);
    List<FuelTypeDto> GetFuelTypes();
    Task<ResponseDto<Vehicle>> UpsertVehicle(UpsertVehicleDto model);
    Task<ResponseDto<Vehicle>> FindByPlate(string plate, string userId);
    Task<ResponseDto<Vehicle>> FindById(string vehicleId, string userId);
}

public sealed class VehicleService(IRepository<Vehicle> repo) : IVehicleService
{
    public async Task<ResponseDto<List<QuickVehicle>>> GetAllVehicles(string userId, CancellationToken ct = default)
    {
        var vehicles = await repo.FindAllAsQueryable(xx => xx.UserId == userId)
            .Select(xx => new QuickVehicle
            {
                Id = xx.Id,
                Name = xx.Name,
                Plate = xx.Plate,
            })
            .ToListAsync(ct);

        var result = new ResponseDto<List<QuickVehicle>>
        {
            Data = vehicles,
            TotalCount = (uint)vehicles.Count
        };

        Log.Logger.Information("Found {@count} vehicles for user {@userId}", result.TotalCount, userId);

        return result;
    }

    public List<FuelTypeDto> GetFuelTypes()
    {
        return [
            new(){ Label="Benzina", Value=FuelType.Gasoline },
            new(){ Label="Diesel", Value=FuelType.Diesel },
            new(){ Label="Elettrica", Value=FuelType.Electric },
            new(){ Label="Ibrida: benzina-elettrica", Value=FuelType.Hybryd_Gasoline_Electric },
        ];
    }

    public async Task<ResponseDto<Vehicle>> UpsertVehicle(UpsertVehicleDto model)
    {
        model = model with { Plate = model.Plate.Sanitize() };

        var onDb = await FindByPlate(model.Plate, model.UserId);
        if (onDb.Data is null) return await AddVehicle(model);

        return await EditVehicle(model, onDb.Data);
    }

    public async Task<ResponseDto<Vehicle>> FindByPlate(string plate, string userId)
    {
        var result = new ResponseDto<Vehicle>
        {
            Data = await repo.FindAsync(xx => xx.Plate == plate && xx.UserId == userId),
        };

        if (result.Data is null)
        {
            Log.Logger.Warning("Vehicle with plate {@plate} not found for user {@userId}", plate, userId);
            result.Errors.Add($"Veicolo con targa {plate} non trovato.");
        }

        return result;
    }

    private async Task<ResponseDto<Vehicle>> AddVehicle(UpsertVehicleDto model)
    {
        var toAdd = new Vehicle
        {
            UserId = model.UserId,
            Plate = model.Plate.Sanitize(),
            Name = model.Name.Sanitize(),
            BuyDate = model.BuyDate,
            FuelType = model.FuelType,
            TotalKm = model.TotalKm,
            InsertDate = DateTime.Now,
            Photo = string.Empty,
            TotalFuel = 0
        };


        await repo.AddAsync(toAdd);

        var result = await FindByPlate(model.Plate, model.UserId);
        if (!result.HasErrors && result.Data is not null)
        {
            Log.Logger.Information("Created vehicle with plate {@plate} and id {@vehicleId} for user {@userId}.", result.Data.Plate, result.Data.Id, result.Data.UserId);
        }

        return result;
    }

    private async Task<ResponseDto<Vehicle>> EditVehicle(UpsertVehicleDto model, Vehicle onDb)
    {
        if (onDb.Id != model.Id)
        {
            Log.Logger.Error("Vehicle with plate {@plate} and id {@vehicleId} not found for user {@userId}.", model.Plate, model.Id, model.UserId);

            return new()
            {
                Errors = [
                    $"Veicolo con targa {model.Plate} non trovato."
                ],
            };
        }

        var updated = onDb with
        {
            UserId = model.UserId,
            Plate = model.Plate,
            Name = model.Name,
            BuyDate = model.BuyDate,
            FuelType = model.FuelType,
            TotalKm = model.TotalKm,
            InsertDate = DateTime.Now,
            Photo = string.Empty,
            TotalFuel = 0
        };

        var result = await repo.UpdateAsync(updated, updated.Id);

        if (result is null)
        {
            Log.Logger.Fatal("Cannot update vehicle with plate {@plate} and id {@vehicleId} for user {@userId}.", model.Plate, model.Id, model.UserId);

            return new()
            {
                Errors = [
                    $"Veicolo con targa {model.Plate} non trovato."
                ],
            };
        }

        Log.Logger.Information("Updated vehicle with plate {@plate} and id {@vehicleId} for user {@userId}.", model.Plate, model.Id, model.UserId);

        return new()
        {
            Data = result
        };
    }

    public async Task<ResponseDto<Vehicle>> FindById(string vehicleId, string userId)
    {
        var result = new ResponseDto<Vehicle>()
        {
            Data = await repo.FindAsync(xx => xx.Id == vehicleId && xx.UserId == userId),
        };

        if(result.Data is null)
        {
            Log.Logger.Error("Vehicle with id {@vehicleId} not found for user {@userId}.", vehicleId, userId);

            result.Errors.Add("Veicolo non trovato.");
        }

        return result;
    }
}
