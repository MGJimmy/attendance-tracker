namespace AttendanceTracker.Contracts
{
    public class AttendanceReportRowDTO
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public DateOnly AttendanceDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? WorkingHours { get; set; }
    }
}
