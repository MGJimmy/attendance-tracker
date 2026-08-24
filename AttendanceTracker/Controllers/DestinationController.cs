using AttendanceTracker.Contracts;
using AttendanceTracker.Domain;
using AttendanceTracker.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Controllers;

[ApiController]
[Route("destination")]
[AllowAnonymous]
public class DestinationController : ControllerBase
{
    private readonly AppDBContext _db;

    public DestinationController(AppDBContext db)
    {
        _db = db;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddDestinationDTO payload)
    {
        var error = ValidateDestination(payload.Name, payload.Details, payload.Cost);
        if (error != null)
            return BadRequest(error);

        var name = payload.Name.Trim();
        var details = (payload.Details ?? string.Empty).Trim();

        if (await _db.Destinations.AnyAsync(x => x.Name.ToLower() == name.ToLower()))
            return BadRequest("A destination with this name already exists.");

        var destination = new Destination
        {
            Name = name,
            Details = details,
            Cost = payload.Cost,
            IsActive = true
        };

        await _db.Destinations.AddAsync(destination);
        await _db.SaveChangesAsync();
        return Ok(destination);
    }

    [HttpPut("edit")]
    public async Task<IActionResult> Edit(EditDestinationDTO payload)
    {
        var destination = await _db.Destinations.FirstOrDefaultAsync(x => x.Id == payload.Id);
        if (destination == null)
            return NotFound("Destination not found.");

        var error = ValidateDestination(payload.Name, payload.Details, payload.Cost);
        if (error != null)
            return BadRequest(error);

        var name = payload.Name.Trim();
        var details = (payload.Details ?? string.Empty).Trim();

        if (await _db.Destinations.AnyAsync(x => x.Id != payload.Id && x.Name.ToLower() == name.ToLower()))
            return BadRequest("Another destination already has this name.");

        destination.Name = name;
        destination.Details = details;
        destination.Cost = payload.Cost;
        await _db.SaveChangesAsync();
        return Ok(destination);
    }

    [HttpPost("activate/{id:int}")]
    public async Task<IActionResult> Activate(int id)
    {
        var destination = await _db.Destinations.FirstOrDefaultAsync(x => x.Id == id);
        if (destination == null)
            return NotFound("Destination not found.");

        if (destination.IsActive)
            return BadRequest("Destination is already active.");

        destination.IsActive = true;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Destination activated successfully." });
    }

    [HttpPost("deactivate/{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var destination = await _db.Destinations.FirstOrDefaultAsync(x => x.Id == id);
        if (destination == null)
            return NotFound("Destination not found.");

        if (!destination.IsActive)
            return BadRequest("Destination is already inactive.");

        destination.IsActive = false;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Destination deactivated successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.Destinations
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Destinations.OrderBy(x => x.Name).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var destination = await _db.Destinations.FirstOrDefaultAsync(x => x.Id == id);
        if (destination == null)
            return NotFound("Destination not found.");
        return Ok(destination);
    }

    private static string? ValidateDestination(string name, string? details, decimal cost)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Destination name is required.";

        if (name.Trim().Length > 100)
            return "Destination name cannot exceed 100 characters.";

        if (!string.IsNullOrEmpty(details) && details.Trim().Length > 500)
            return "Details cannot exceed 500 characters.";

        if (cost < 0)
            return "Cost cannot be negative.";

        return null;
    }
}
