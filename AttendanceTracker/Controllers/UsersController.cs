using AttendanceTracker.Infrastructure;
using Identity.Service;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Controllers;

[ApiController]
[Route("users")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AppDBContext _appDBContext;
    private readonly IIdmProvider<User> _idmProvider;

    public UsersController(IMediator mediator, AppDBContext appDBContext, IIdmProvider<User> idmProvider)
    {
        _mediator = mediator;
        _appDBContext = appDBContext;
        _idmProvider = idmProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetAlluserQuery());
        return Ok(users);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] int? employeeId)
    {
        var users = await _mediator.Send(new GetAlluserQuery());

        var takenUserIds = await _appDBContext.Employees
            .Where(employee => employeeId == null || employee.Id != employeeId.Value)
            .Select(employee => employee.UserId)
            .ToListAsync();

        var available = users
            .Where(user => user.IsActive && !takenUserIds.Contains(user.Id))
            .ToList();

        return Ok(available);
    }

    [HttpPost("activate/{id}")]
    public async Task<IActionResult> Activate(string id)
    {
        var result = await _idmProvider.SetActiveAsync(id, true);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(new { message = "User activated successfully." });
    }

    [HttpPost("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(string id)
    {
        var result = await _idmProvider.SetActiveAsync(id, false);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        var employee = await _appDBContext.Employees.FirstOrDefaultAsync(x => x.UserId == id);
        if (employee != null && employee.IsActive)
        {
            employee.IsActive = false;
            await _appDBContext.SaveChangesAsync();
        }

        return Ok(new { message = "User deactivated successfully." });
    }
}
