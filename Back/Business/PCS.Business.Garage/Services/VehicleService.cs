using PCS.Common.Entities.Models.Entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using PCS.Common.Entities.Models.Dtos;
using PCS.Business.Garage.Models.DTOs;
using PCS.Common.Business.Extensions;
using PCS.Common.Business;

namespace PCS.Business.Garage.Services;

public interface IVehicleService
{
    Task<ResponseDto<List<QuickVehicle>>> GetAllVehicles(string userId, CancellationToken ct = default);
    List<FuelTypeDto> GetFuelTypes();
    Task<ResponseDto<Vehicle>> UpsertVehicle(UpsertVehicleDto model);
    Task<ResponseDto<Vehicle>> FindByPlate(string plate, string userId);
    Task<ResponseDto<Vehicle>> FindById(string vehicleId, string userId);
    Task<ResponseDto<QuickVehicle>> FindQuickById(string vehicleId, string userId);
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

        var onDb = await FindById(model.Id, model.UserId);
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

        return result;
    }

    private async Task<ResponseDto<Vehicle>> EditVehicle(UpsertVehicleDto model, Vehicle onDb)
    {
        var updated = onDb with
        {
            UserId = model.UserId,
            Plate = model.Plate,
            Name = model.Name,
            BuyDate = model.BuyDate,
            FuelType = model.FuelType,
            TotalKm = model.TotalKm,
            InsertDate = DateTime.Now,
        };

        var result = await repo.UpdateAsync(updated, updated.Id);

        if (result is null)
        {
            return new()
            {
                Errors = [
                    $"Veicolo con targa {model.Plate} non trovato."
                ],
            };
        }

        return new()
        {
            Data = result
        };
    }

    public async Task<ResponseDto<Vehicle>> FindById(string? vehicleId, string userId)
    {
        var result = new ResponseDto<Vehicle>()
        {
            Data = await repo.FindAsync(xx => xx.Id == vehicleId && xx.UserId == userId),
        };

        if (result.Data is null)
        {
            result.Errors.Add("Veicolo non trovato.");
        }

        return result;
    }
    public async Task<ResponseDto<QuickVehicle>> FindQuickById(string vehicleId, string userId)
    {
        var vehicle = (await FindById(vehicleId, userId))?.Data;

        var result = new ResponseDto<QuickVehicle>()
        {
            Data = vehicle is null
                ? null
                : new()
                {
                    Id = vehicle.Id,
                    Name = vehicle.Name,
                    Plate = vehicle.Plate,
                }
        };

        if (result.Data is null)
        {
            result.Errors.Add("Veicolo non trovato.");
        }

        return result;
    }
}
