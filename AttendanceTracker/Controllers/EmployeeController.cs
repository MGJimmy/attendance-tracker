using AttendanceTracker.Contracts;
using AttendanceTracker.Domain;
using AttendanceTracker.Infrastructure;
using Identity.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceTracker.Controllers;

[ApiController]
[Route("employee")]
[AllowAnonymous]
public class EmployeeController : ControllerBase
{
    private readonly AppDBContext _appDBContext;
    private readonly UserManager<User> _userManager;
    private readonly IUserContextService _userContext;

    public EmployeeController(
        AppDBContext appDBContext,
        UserManager<User> userManager,
        IUserContextService userContext)
    {
        _appDBContext = appDBContext;
        _userManager = userManager;
        _userContext = userContext;
    }

    #region Add

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddEmployeeDTO payload)
    {
        if (string.IsNullOrWhiteSpace(payload.Name))
            return BadRequest("Employee name is required.");

        if (payload.SalaryPerHour <= 0)
            return BadRequest("Salary per hour must be greater than zero.");

        payload.Name = payload.Name.Trim();

        var userError = await ValidateUserLinkAsync(payload.UserId);
        if (userError != null)
            return BadRequest(userError);

        var exists = await _appDBContext.Employees
            .AnyAsync(x => x.Name.ToLower() == payload.Name.ToLower());

        if (exists)
            return BadRequest("Employee already exists.");

        var employee = new Employee
        {
            Name = payload.Name,
            SalaryPerHour = payload.SalaryPerHour,
            UserId = payload.UserId.Trim(),
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

        if (payload.SalaryPerHour <= 0)
            return BadRequest("Salary per hour must be greater than zero.");

        payload.Name = payload.Name.Trim();

        var userError = await ValidateUserLinkAsync(payload.UserId, payload.Id);
        if (userError != null)
            return BadRequest(userError);

        var duplicate = await _appDBContext.Employees
            .AnyAsync(x =>
                x.Id != payload.Id &&
                x.Name.ToLower() == payload.Name.ToLower());

        if (duplicate)
            return BadRequest("Another employee already has this name.");

        employee.Name = payload.Name;
        employee.SalaryPerHour = payload.SalaryPerHour;
        employee.UserId = payload.UserId.Trim();

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

        var today = EgyptTime.Today;

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

    #region Get Current Employee

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentEmployee()
    {
        var userId = _userContext.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User is not logged in.");

        var employee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (employee == null)
            return NotFound("No employee is linked to this user.");

        return Ok(employee);
    }

    #endregion

    #region Check In

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(CheckInDTO payload)
    {
        var occurredAtUtc = ResolveOccurredAt(payload.OccurredAt);
        var attendanceDate = EgyptTime.ToDate(occurredAtUtc);

        var employee = await ResolveEmployeeAsync(payload.EmployeeId);
        if (employee.result != null)
            return employee.result;

        var employeeEntity = employee.employee!;

        if (!employeeEntity.IsActive)
            return BadRequest("Employee is inactive.");

        var attendance = await _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == employeeEntity.Id &&
                x.AttendanceDate == attendanceDate);

        if (attendance != null)
            return BadRequest("Employee has already checked in on this date.");

        attendance = new EmployeeAttendance
        {
            EmployeeId = employeeEntity.Id,
            AttendanceDate = attendanceDate,
            CheckInTime = occurredAtUtc
        };

        await _appDBContext.EmployeeAttendances.AddAsync(attendance);
        await _appDBContext.SaveChangesAsync();

        return Ok(new
        {
            attendance.Id,
            attendance.EmployeeId,
            employeeEntity.Name,
            attendance.AttendanceDate,
            attendance.CheckInTime,
            attendance.CheckOutTime,
            WorkingHours = attendance.WorkingHours
        });
    }

    #endregion

    #region Check Out

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(CheckOutDTO payload)
    {
        var occurredAtUtc = ResolveOccurredAt(payload.OccurredAt);
        var attendanceDate = EgyptTime.ToDate(occurredAtUtc);

        var employee = await ResolveEmployeeAsync(payload.EmployeeId);
        if (employee.result != null)
            return employee.result;

        var employeeEntity = employee.employee!;

        var attendance = await _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == employeeEntity.Id &&
                x.AttendanceDate == attendanceDate);

        if (attendance == null)
        {
            attendance = await _appDBContext.EmployeeAttendances
                .Include(x => x.Employee)
                .Where(x =>
                    x.EmployeeId == employeeEntity.Id &&
                    x.CheckInTime != null &&
                    x.CheckOutTime == null)
                .OrderByDescending(x => x.CheckInTime)
                .FirstOrDefaultAsync();
        }

        if (attendance == null)
            return BadRequest("Employee didn't check in.");

        if (attendance.CheckOutTime != null)
            return BadRequest("Employee already checked out.");

        if (occurredAtUtc <= attendance.CheckInTime)
            return BadRequest("Check-out time must be after check-in time.");

        attendance.CheckOutTime = occurredAtUtc;

        await _appDBContext.SaveChangesAsync();

        return Ok(new
        {
            attendance.Id,
            attendance.EmployeeId,
            attendance.Employee.Name,
            attendance.AttendanceDate,
            attendance.CheckInTime,
            attendance.CheckOutTime,
            WorkingHours = attendance.WorkingHours
        });
    }

    #endregion

    #region Edit Attendance

    [HttpPut("attendance/{id:int}")]
    public async Task<IActionResult> EditAttendance(int id, EditAttendanceDTO payload)
    {
        if (!_userContext.IsAdmin())
            return StatusCode(StatusCodes.Status403Forbidden, "Only an admin can edit attendance.");

        var attendance = await _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (attendance == null)
            return NotFound("Attendance record not found.");

        var checkInUtc = EgyptTime.ToUtc(payload.CheckInTime);
        DateTime? checkOutUtc = payload.CheckOutTime.HasValue
            ? EgyptTime.ToUtc(payload.CheckOutTime.Value)
            : null;

        if (checkOutUtc.HasValue && checkOutUtc <= checkInUtc)
            return BadRequest("Check-out time must be after check-in time.");

        var attendanceDate = EgyptTime.ToDate(checkInUtc);

        var conflict = await _appDBContext.EmployeeAttendances
            .AnyAsync(x =>
                x.Id != id &&
                x.EmployeeId == attendance.EmployeeId &&
                x.AttendanceDate == attendanceDate);

        if (conflict)
            return BadRequest("Another attendance record already exists for this employee on that date.");

        attendance.CheckInTime = checkInUtc;
        attendance.CheckOutTime = checkOutUtc;
        attendance.AttendanceDate = attendanceDate;

        await _appDBContext.SaveChangesAsync();

        return Ok(new
        {
            attendance.Id,
            attendance.EmployeeId,
            attendance.Employee.Name,
            attendance.AttendanceDate,
            attendance.CheckInTime,
            attendance.CheckOutTime,
            WorkingHours = attendance.WorkingHours
        });
    }

    #endregion

    #region Today Attendance

    [HttpGet("today")]
    public async Task<IActionResult> TodayAttendance()
    {
        var today = EgyptTime.Today;

        var query = _appDBContext.EmployeeAttendances
            .Include(x => x.Employee)
            .Where(x => x.AttendanceDate == today);

        if (!_userContext.IsAdmin())
        {
            var userId = _userContext.GetCurrentUserId();
            query = query.Where(x => x.Employee.UserId == userId);
        }

        var result = await query
            .OrderBy(x => x.Employee.Name)
            .Select(x => new
            {
                x.Id,
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

    [HttpPost("history")]
    public async Task<IActionResult> History([FromBody] AttendanceHistoryDTO payload)
    {
        IQueryable<EmployeeAttendance> query = _appDBContext.EmployeeAttendances
            .Include(x => x.Employee);

        if (payload.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == payload.EmployeeId.Value);
        }

        if (payload.From.HasValue)
        {
            query = query.Where(x => x.AttendanceDate >= payload.From.Value);
        }

        if (payload.To.HasValue)
        {
            query = query.Where(x => x.AttendanceDate <= payload.To.Value);
        }

        var result = await query
            .OrderByDescending(x => x.AttendanceDate)
            .ThenBy(x => x.Employee.Name)
            .Select(x => new
            {
                x.Id,
                x.EmployeeId,
                Name = x.Employee.Name,
                x.AttendanceDate,
                x.CheckInTime,
                x.CheckOutTime,
                WorkingHours = x.CheckOutTime == null
                    ? null
                    : x.CheckOutTime - x.CheckInTime
            })
            .ToListAsync();

        return Ok(result);
    }

    #endregion

    #region Reports

    [HttpPost("report")]
    public async Task<IActionResult> Report([FromBody] AttendanceReportDTO payload)
    {
        IQueryable<EmployeeAttendance> query =
            _appDBContext.EmployeeAttendances
                .Include(x => x.Employee);

        if (payload.EmployeeId.HasValue)
        {
            query = query.Where(x =>
                x.EmployeeId == payload.EmployeeId.Value);
        }

        if (payload.From.HasValue)
        {
            query = query.Where(x =>
                x.AttendanceDate >= payload.From.Value);
        }

        if (payload.To.HasValue)
        {
            query = query.Where(x =>
                x.AttendanceDate <= payload.To.Value);
        }

        var attendances = await query
            .OrderByDescending(x => x.AttendanceDate)
            .ThenBy(x => x.Employee.Name)
            .ToListAsync();

        var rows = attendances
            .Select(x => new AttendanceReportRowDTO
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.Name,
                AttendanceDate = x.AttendanceDate,
                CheckInTime = x.CheckInTime,
                CheckOutTime = x.CheckOutTime,
                WorkingHours = x.WorkingHours
            })
            .ToList();

        var completedAttendances = attendances
            .Where(x => x.WorkingHours.HasValue)
            .ToList();

        var totalWorkingHours = new TimeSpan(
            completedAttendances.Sum(x => x.WorkingHours!.Value.Ticks));

        var averageWorkingHours =
            completedAttendances.Count == 0
                ? TimeSpan.Zero
                : new TimeSpan(
                    totalWorkingHours.Ticks / completedAttendances.Count);

        var longest = completedAttendances
            .OrderByDescending(x => x.WorkingHours)
            .FirstOrDefault();

        var shortest = completedAttendances
            .OrderBy(x => x.WorkingHours)
            .FirstOrDefault();

        decimal? salaryPerHour = null;
        if (payload.EmployeeId.HasValue)
        {
            salaryPerHour = attendances.FirstOrDefault()?.Employee.SalaryPerHour
                ?? (await _appDBContext.Employees.FirstOrDefaultAsync(x => x.Id == payload.EmployeeId.Value))?.SalaryPerHour;
        }
        else
        {
            var distinctRates = attendances
                .Select(x => x.Employee.SalaryPerHour)
                .Distinct()
                .ToList();

            if (distinctRates.Count == 1)
                salaryPerHour = distinctRates[0];
        }

        var totalSalary = completedAttendances.Sum(x =>
            (decimal)x.WorkingHours!.Value.TotalHours * x.Employee.SalaryPerHour);

        var result = new AttendanceReportResultDTO
        {
            WorkingDays = rows.Count,

            CompletedDays = completedAttendances.Count,

            OpenDays = rows.Count(x => x.CheckOutTime == null),

            TotalWorkingHours = totalWorkingHours,

            AverageWorkingHours = averageWorkingHours,

            EarliestCheckIn = rows
                .Where(x => x.CheckInTime.HasValue)
                .OrderBy(x => x.CheckInTime)
                .Select(x => x.CheckInTime)
                .FirstOrDefault(),

            LatestCheckOut = rows
                .Where(x => x.CheckOutTime.HasValue)
                .OrderByDescending(x => x.CheckOutTime)
                .Select(x => x.CheckOutTime)
                .FirstOrDefault(),

            LongestShift = longest?.WorkingHours,
            LongestShiftDate = longest?.AttendanceDate,

            ShortestShift = shortest?.WorkingHours,
            ShortestShiftDate = shortest?.AttendanceDate,

            SalaryPerHour = salaryPerHour,
            TotalSalary = Math.Round(totalSalary, 2),

            Records = rows
        };

        return Ok(result);
    }
  

    #endregion


    #region Absent Employees Today

    [HttpGet("today-absent")]
    public async Task<IActionResult> TodayAbsent()
    {
        var today = EgyptTime.Today;

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
        var today = EgyptTime.Today;

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
        var today = EgyptTime.Today;

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

    private DateTime ResolveOccurredAt(DateTime? occurredAt)
    {
        if (occurredAt.HasValue && _userContext.IsAdmin())
            return EgyptTime.ToUtc(occurredAt.Value);

        return EgyptTime.UtcNow;
    }

    private async Task<(Employee? employee, IActionResult? result)> ResolveEmployeeAsync(int? employeeId)
    {
        if (_userContext.IsAdmin())
        {
            if (!employeeId.HasValue)
                return (null, BadRequest("Employee is required."));

            var adminEmployee = await _appDBContext.Employees
                .FirstOrDefaultAsync(x => x.Id == employeeId.Value);

            if (adminEmployee == null)
                return (null, NotFound("Employee not found."));

            return (adminEmployee, null);
        }

        var userId = _userContext.GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return (null, Unauthorized("User is not logged in."));

        var linkedEmployee = await _appDBContext.Employees
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (linkedEmployee == null)
            return (null, BadRequest("No employee is linked to this user."));

        return (linkedEmployee, null);
    }

    private async Task<string?> ValidateUserLinkAsync(string userId, int? employeeId = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return "A user must be selected.";

        userId = userId.Trim();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return "Selected user was not found.";

        var alreadyLinked = await _appDBContext.Employees
            .AnyAsync(employee =>
                employee.UserId == userId &&
                (employeeId == null || employee.Id != employeeId.Value));

        if (alreadyLinked)
            return "This user is already linked to another employee.";

        return null;
    }

}