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

    public UsersController(IMediator mediator, AppDBContext appDBContext)
    {
        _mediator = mediator;
        _appDBContext = appDBContext;
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
            .Where(user => !takenUserIds.Contains(user.Id))
            .ToList();

        return Ok(available);
    }
}
