using AttendanceTracker.Contracts;
using AttendanceTracker.Domain;
using AttendanceTracker.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Controllers;

[ApiController]
[Route("courier")]
[AllowAnonymous]
public class CourierController : ControllerBase
{
    private readonly AppDBContext _db;

    public CourierController(AppDBContext db)
    {
        _db = db;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddCourierDTO payload)
    {
        var error = ValidateCourier(payload.Name, payload.MobileNumber);
        if (error != null)
            return BadRequest(error);

        var name = payload.Name.Trim();
        var mobile = payload.MobileNumber.Trim();

        if (await _db.Couriers.AnyAsync(x => x.Name.ToLower() == name.ToLower()))
            return BadRequest("A courier with this name already exists.");

        if (await _db.Couriers.AnyAsync(x => x.MobileNumber == mobile))
            return BadRequest("A courier with this mobile number already exists.");

        var courier = new Courier
        {
            Name = name,
            MobileNumber = mobile,
            IsActive = true
        };

        await _db.Couriers.AddAsync(courier);
        await _db.SaveChangesAsync();
        return Ok(courier);
    }

    [HttpPut("edit")]
    public async Task<IActionResult> Edit(EditCourierDTO payload)
    {
        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == payload.Id);
        if (courier == null)
            return NotFound("Courier not found.");

        var error = ValidateCourier(payload.Name, payload.MobileNumber);
        if (error != null)
            return BadRequest(error);

        var name = payload.Name.Trim();
        var mobile = payload.MobileNumber.Trim();

        if (await _db.Couriers.AnyAsync(x => x.Id != payload.Id && x.Name.ToLower() == name.ToLower()))
            return BadRequest("Another courier already has this name.");

        if (await _db.Couriers.AnyAsync(x => x.Id != payload.Id && x.MobileNumber == mobile))
            return BadRequest("Another courier already has this mobile number.");

        courier.Name = name;
        courier.MobileNumber = mobile;
        await _db.SaveChangesAsync();
        return Ok(courier);
    }

    [HttpPost("activate/{id:int}")]
    public async Task<IActionResult> Activate(int id)
    {
        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == id);
        if (courier == null)
            return NotFound("Courier not found.");

        if (courier.IsActive)
            return BadRequest("Courier is already active.");

        courier.IsActive = true;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Courier activated successfully." });
    }

    [HttpPost("deactivate/{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == id);
        if (courier == null)
            return NotFound("Courier not found.");

        if (!courier.IsActive)
            return BadRequest("Courier is already inactive.");

        courier.IsActive = false;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Courier deactivated successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.Couriers
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Couriers.OrderBy(x => x.Name).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == id);
        if (courier == null)
            return NotFound("Courier not found.");
        return Ok(courier);
    }

    [HttpGet("{id:int}/summary")]
    public async Task<IActionResult> GetSummary(int id, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
    {
        var courier = await _db.Couriers.FirstOrDefaultAsync(x => x.Id == id);
        if (courier == null)
            return NotFound("Courier not found.");

        var query = _db.Trips
            .Include(x => x.Destination)
            .Where(x => x.CourierId == id);

        if (from.HasValue)
            query = query.Where(x => x.TripDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.TripDate <= to.Value);

        var trips = await query
            .OrderByDescending(x => x.TripDate)
            .ThenByDescending(x => x.Id)
            .Select(x => new CourierTripRowDTO
            {
                Id = x.Id,
                DestinationName = x.Destination.Name,
                TripDate = x.TripDate,
                Cost = x.Cost
            })
            .ToListAsync();

        return Ok(new CourierSummaryDTO
        {
            Id = courier.Id,
            Name = courier.Name,
            MobileNumber = courier.MobileNumber,
            IsActive = courier.IsActive,
            TotalTrips = trips.Count,
            TotalPay = trips.Sum(x => x.Cost),
            Trips = trips
        });
    }

    private static string? ValidateCourier(string name, string mobile)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Courier name is required.";

        if (name.Trim().Length > 100)
            return "Courier name cannot exceed 100 characters.";

        if (string.IsNullOrWhiteSpace(mobile))
            return "Mobile number is required.";

        if (mobile.Trim().Length > 30)
            return "Mobile number cannot exceed 30 characters.";

        return null;
    }
}
