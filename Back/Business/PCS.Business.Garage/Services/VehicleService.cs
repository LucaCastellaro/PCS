using PCS.Common.Entities.Models.Entities;
using SpRepo.Abstraction;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using PCS.Common.Entities.Models.Dtos;
using PCS.Business.Garage.Models.DTOs;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using PCS.Common.Business.Extensions;

namespace PCS.Business.Garage.Services;

public interface IVehicleService
{
    Task<List<QuickVehicle>> GetAllVehicles(string userId, CancellationToken ct = default);
    List<FuelTypeDto> GetFuelTypes();
    Task<ResponseDto<Vehicle>> UpsertVehicle(UpsertVehicleDto model);
    Task<Vehicle?> FindByPlate(string plate, string userId);
}

public class VehicleService(IRepository<Vehicle> repo,
    ILogger<VehicleService> logger
    ) : IVehicleService
{
    public async Task<List<QuickVehicle>> GetAllVehicles(string userId, CancellationToken ct = default)
    {
        return await repo.FindAllAsQueryable(xx => xx.UserId == userId)
            .Select(xx => new QuickVehicle
            {
                Id = xx.Id,
                Name = xx.Name,
                Plate = xx.Plate,
            })
            .ToListAsync(ct);
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
        var onDb = await FindByPlate(model.Plate, model.UserId);
        if (onDb is null) return await AddVehicle(model);

        return await EditVehicle(model, onDb);
    }

    public Task<Vehicle?> FindByPlate(string plate, string userId)
    {
        return repo.FindAsync(xx => xx.Plate == plate && xx.UserId == userId);
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
        if (result is null)
        {
            logger.LogError("Errore nella creazione del veicolo con targa {@plate} sull'utente {@userId}.", model.Plate, model.UserId);

            return new()
            {
                Errors = [
                    $"Veicolo con targa {model.Plate} non trovato."
                ],
            };
        }

        logger.LogInformation("Creato veicolo con targa {@plate} e id {@vehicleId} sull'utente {@userId}.", result.Plate, result.Id, model.UserId);
        return new()
        {
            Data = toAdd
        };
    }

    private async Task<ResponseDto<Vehicle>> EditVehicle(UpsertVehicleDto model, Vehicle onDb)
    {
        if (onDb.Id != model.Id)
        {
            logger.LogError("Veicolo con targa {@plate} e id {@vehicleId} non trovato sull'utente {@userId}.", model.Plate, model.Id, model.UserId);

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
            logger.LogCritical("Errore nell'aggiornamento del veicolo con targa {@plate} e id {@vehicleId} non trovato sull'utente {@userId}.", model.Plate, model.Id, model.UserId);

            return new()
            {
                Errors = [
                    $"Veicolo con targa {model.Plate} non trovato."
                ],
            };
        }

        logger.LogInformation("Aggiornato veicolo con targa {@plate} e id {@vehicleId} sull'utente {@userId}.", model.Plate, model.Id, model.UserId);

        return new()
        {
            Data = result
        };
    }
}
