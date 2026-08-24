namespace AttendanceTracker.Contracts;

public class EditEmployeeDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal SalaryPerHour { get; set; }

    public string UserId { get; set; } = string.Empty;
}
