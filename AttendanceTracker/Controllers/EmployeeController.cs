using AttendanceTracker.Contracts;
using AttendanceTracker.Domain;
using AttendanceTracker.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Controllers;

[ApiController]
[Route("employee")]
[AllowAnonymous]
public class EmployeeController : ControllerBase
{
    private readonly AppDBContext _appDBContext;

    public EmployeeController(AppDBContext appDBContext)
    {
        _appDBContext = appDBContext;
    }

    #region Add

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddEmployeeDTO payload)
    {
        if (string.IsNullOrWhiteSpace(payload.Name))
            return BadRequest("Employee name is required.");

        if (payload.Salary <= 0)
            return BadRequest("Salary must be greater than zero.");

        payload.Name = payload.Name.Trim();

        var exists = await _appDBContext.Employees
            .AnyAsync(x => x.Name.ToLower() == payload.Name.ToLower());

        if (exists)
            return BadRequest("Employee already exists.");

        var employee = new Employee
        {
            Name = payload.Name,
            Salary = payload.Salary,
            IsActive = true
        };

        await _appDBContext.Employees.AddAsync(employee);
        await _appDBContext.SaveChangesAsync();

        return Ok(employee);
    }

    #endregion

    #region Edit

    [HttpPut("edit")]
    public async Task<IActionResult> Edit(EditEmployeeDTO payload)
    {
        var employee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.Id == payload.Id);

        if (employee == null)
            return NotFound("Employee not found.");

        if (string.IsNullOrWhiteSpace(payload.Name))
            return BadRequest("Employee name is required.");

        if (payload.Salary <= 0)
            return BadRequest("Salary must be greater than zero.");

        payload.Name = payload.Name.Trim();

        var duplicate = await _appDBContext.Employees
            .AnyAsync(x =>
                x.Id != payload.Id &&
                x.Name.ToLower() == payload.Name.ToLower());

        if (duplicate)
            return BadRequest("Another employee already has this name.");

        employee.Name = payload.Name;
        employee.Salary = payload.Salary;

        await _appDBContext.SaveChangesAsync();

        return Ok(employee);
    }

    #endregion

    #region Activate

    [HttpPost("activate/{id:int}")]
    public async Task<IActionResult> Activate(int id)
    {
        var employee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound("Employee not found.");

        if (employee.IsActive)
            return BadRequest("Employee is already active.");

        employee.IsActive = true;

        await _appDBContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Employee activated successfully."
        });
    }

    #endregion

    #region Deactivate

    [HttpPost("deactivate/{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var employee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound("Employee not found.");

        if (!employee.IsActive)
            return BadRequest("Employee is already inactive.");

        var today = DateOnly.FromDateTime(DateTime.Today);

        var openedAttendance = await _appDBContext.EmployeeAttendances
            .AnyAsync(x =>
                x.EmployeeId == id &&
                x.AttendanceDate == today &&
                x.CheckInTime != null &&
                x.CheckOutTime == null);

        if (openedAttendance)
            return BadRequest("Employee is currently checked in.");

        employee.IsActive = false;

        await _appDBContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Employee deactivated successfully."
        });
    }

    #endregion

    #region Get Active Employees

    [HttpGet]
    public async Task<IActionResult> GetActiveEmployees()
    {
        var employees = await _appDBContext.Employees
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(employees);
    }

    #endregion

    #region Get All Employees

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _appDBContext.Employees
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(employees);
    }

    #endregion

    #region Get Employee By Id

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound("Employee not found.");

        return Ok(employee);
    }

    #endregion

    #region Check In

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(CheckInDTO payload)
    {
        var employee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.Id == payload.EmployeeId);

        if (employee == null)
            return NotFound("Employee not found.");

        if (!employee.IsActive)
            return BadRequest("Employee is inactive.");

        var today = DateOnly.FromDateTime(DateTime.Today);

        var attendance = await _appDBContext.EmployeeAttendances
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == payload.EmployeeId &&
                x.AttendanceDate == today);

        if (attendance != null)
            return BadRequest("Employee has already checked in today.");

        attendance = new EmployeeAttendance
        {
            EmployeeId = payload.EmployeeId,
            AttendanceDate = today,
            CheckInTime = DateTime.Now
        };

        await _appDBContext.EmployeeAttendances.AddAsync(attendance);
        await _appDBContext.SaveChangesAsync();

        return Ok(attendance);
    }

    #endregion

    #region Check Out

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(CheckOutDTO payload)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var attendance = await _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == payload.EmployeeId &&
                x.AttendanceDate == today);

        if (attendance == null)
            return BadRequest("Employee didn't check in today.");

        if (attendance.CheckOutTime != null)
            return BadRequest("Employee already checked out.");

        attendance.CheckOutTime = DateTime.Now;

        await _appDBContext.SaveChangesAsync();

        return Ok(new
        {
            attendance.EmployeeId,
            attendance.Employee.Name,
            attendance.CheckInTime,
            attendance.CheckOutTime,
            WorkingHours = attendance.CheckOutTime - attendance.CheckInTime
        });
    }

    #endregion

    #region Today Attendance

    [HttpGet("today")]
    public async Task<IActionResult> TodayAttendance()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var result = await _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .Where(x => x.AttendanceDate == today)
            .OrderBy(x => x.Employee.Name)
            .Select(x => new
            {
                x.EmployeeId,
                x.Employee.Name,
                x.CheckInTime,
                x.CheckOutTime,
                WorkingHours =
                    x.CheckOutTime == null
                        ? null
                        : x.CheckOutTime - x.CheckInTime
            })
            .ToListAsync();

        return Ok(result);
    }

    #endregion

    #region Employee Attendance History

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> EmployeeHistory(int employeeId)
    {
        var employee = await _appDBContext.Employees
            .AnyAsync(x => x.Id == employeeId);

        if (!employee)
            return NotFound("Employee not found.");

        var result = await _appDBContext.EmployeeAttendances
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.AttendanceDate)
            .Select(x => new
            {
                x.AttendanceDate,
                x.CheckInTime,
                x.CheckOutTime,
                WorkingHours =
                    x.CheckOutTime == null
                        ? null
                        : x.CheckOutTime - x.CheckInTime
            })
            .ToListAsync();

        return Ok(result);
    }

    #endregion

    #region Attendance Between Dates

    [HttpPost("report")]
    public async Task<IActionResult> Report(AttendanceReportDTO payload)
    {
        var query = _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .AsQueryable();

        if (payload.EmployeeId.HasValue)
            query = query.Where(x => x.EmployeeId == payload.EmployeeId);

        if (payload.From.HasValue)
            query = query.Where(x => x.AttendanceDate >= payload.From);

        if (payload.To.HasValue)
            query = query.Where(x => x.AttendanceDate <= payload.To);

        var result = await query
            .OrderByDescending(x => x.AttendanceDate)
            .Select(x => new
            {
                x.EmployeeId,
                x.Employee.Name,
                x.AttendanceDate,
                x.CheckInTime,
                x.CheckOutTime,
                WorkingHours =
                    x.CheckOutTime == null
                        ? null
                        : x.CheckOutTime - x.CheckInTime
            })
            .ToListAsync();

        return Ok(result);
    }

    #endregion

    #region Absent Employees Today

    [HttpGet("today-absent")]
    public async Task<IActionResult> TodayAbsent()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var result = await _appDBContext.Employees
            .Where(x =>
                x.IsActive &&
                !_appDBContext.EmployeeAttendances.Any(a =>
                    a.EmployeeId == x.Id &&
                    a.AttendanceDate == today))
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(result);
    }

    #endregion

    #region Currently Working

    [HttpGet("currently-working")]
    public async Task<IActionResult> CurrentlyWorking()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var result = await _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .Where(x =>
                x.AttendanceDate == today &&
                x.CheckInTime != null &&
                x.CheckOutTime == null)
            .OrderBy(x => x.Employee.Name)
            .Select(x => new
            {
                x.EmployeeId,
                x.Employee.Name,
                x.CheckInTime
            })
            .ToListAsync();

        return Ok(result);
    }

    #endregion

    #region Dashboard

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var totalEmployees = await _appDBContext.Employees.CountAsync();

        var activeEmployees = await _appDBContext.Employees
            .CountAsync(x => x.IsActive);

        var checkedIn = await _appDBContext.EmployeeAttendances
            .CountAsync(x =>
                x.AttendanceDate == today &&
                x.CheckInTime != null);

        var checkedOut = await _appDBContext.EmployeeAttendances
            .CountAsync(x =>
                x.AttendanceDate == today &&
                x.CheckOutTime != null);

        var absent = activeEmployees - checkedIn;

        return Ok(new
        {
            TotalEmployees = totalEmployees,
            ActiveEmployees = activeEmployees,
            CheckedIn = checkedIn,
            CheckedOut = checkedOut,
            Absent = absent
        });
    }

    #endregion

}