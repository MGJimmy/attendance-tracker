namespace AttendanceTracker.Contracts;

public class AttendanceReportDTO
{
    public int? EmployeeId { get; set; }

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }
}