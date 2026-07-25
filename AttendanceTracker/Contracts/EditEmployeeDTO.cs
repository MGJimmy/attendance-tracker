namespace AttendanceTracker.Contracts;

public class EditEmployeeDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Salary { get; set; }
}