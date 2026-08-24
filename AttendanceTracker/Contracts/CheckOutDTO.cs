namespace AttendanceTracker.Contracts;

public class CheckOutDTO
{
    public int? EmployeeId { get; set; }

    public DateTime? OccurredAt { get; set; }
}