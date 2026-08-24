using AttendanceTracker.Contracts;
using AttendanceTracker.Domain;
using AttendanceTracker.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Controllers;

[ApiController]
[Route("trip")]
[AllowAnonymous]
public class TripController : ControllerBase
{
    private readonly AppDBContext _db;

    public TripController(AppDBContext db)
    {
        _db = db;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddTripDTO payload)
    {
        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == payload.CourierId);
        if (courier == null)
            return NotFound("Courier not found.");

        if (!courier.IsActive)
            return BadRequest("Courier is inactive.");

        var destination = await _db.Destinations.FirstOrDefaultAsync(x => x.Id == payload.DestinationId);
        if (destination == null)
            return NotFound("Destination not found.");

        if (!destination.IsActive)
            return BadRequest("Destination is inactive.");

        var cost = payload.Cost ?? destination.Cost;
        if (cost < 0)
            return BadRequest("Cost cannot be negative.");

        var trip = new Trip
        {
            CourierId = courier.Id,
            DestinationId = destination.Id,
            Cost = cost,
            TripDate = payload.TripDate
        };

        await _db.Trips.AddAsync(trip);
        await _db.SaveChangesAsync();

        return Ok(await MapTrip(trip.Id));
    }

    [HttpPut("edit")]
    public async Task<IActionResult> Edit(EditTripDTO payload)
    {
        var trip = await _db.Trips.FirstOrDefaultAsync(x => x.Id == payload.Id);
        if (trip == null)
            return NotFound("Trip not found.");

        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == payload.CourierId);
        if (courier == null)
            return NotFound("Courier not found.");

        var destination = await _db.Destinations.FirstOrDefaultAsync(x => x.Id == payload.DestinationId);
        if (destination == null)
            return NotFound("Destination not found.");

        if (payload.Cost < 0)
            return BadRequest("Cost cannot be negative.");

        trip.CourierId = courier.Id;
        trip.DestinationId = destination.Id;
        trip.Cost = payload.Cost;
        trip.TripDate = payload.TripDate;

        await _db.SaveChangesAsync();
        return Ok(await MapTrip(trip.Id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? courierId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
    {
        var query = _db.Trips
            .Include(x => x.Courier)
            .Include(x => x.Destination)
            .AsQueryable();

        if (courierId.HasValue)
            query = query.Where(x => x.CourierId == courierId.Value);

        if (from.HasValue)
            query = query.Where(x => x.TripDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.TripDate <= to.Value);

        var items = await query
            .OrderByDescending(x => x.TripDate)
            .ThenBy(x => x.Courier.Name)
            .Select(x => new TripListDTO
            {
                Id = x.Id,
                CourierId = x.CourierId,
                CourierName = x.Courier.Name,
                DestinationId = x.DestinationId,
                DestinationName = x.Destination.Name,
                Cost = x.Cost,
                TripDate = x.TripDate
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trip = await MapTrip(id);
        if (trip == null)
            return NotFound("Trip not found.");
        return Ok(trip);
    }

    private async Task<TripListDTO?> MapTrip(int id)
    {
        return await _db.Trips
            .Include(x => x.Courier)
            .Include(x => x.Destination)
            .Where(x => x.Id == id)
            .Select(x => new TripListDTO
            {
                Id = x.Id,
                CourierId = x.CourierId,
                CourierName = x.Courier.Name,
                DestinationId = x.DestinationId,
                DestinationName = x.Destination.Name,
                Cost = x.Cost,
                TripDate = x.TripDate
            })
            .FirstOrDefaultAsync();
    }
}
