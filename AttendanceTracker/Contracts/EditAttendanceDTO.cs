namespace AttendanceTracker.Contracts;

public class EditAttendanceDTO
{
    public DateTime CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }
}
