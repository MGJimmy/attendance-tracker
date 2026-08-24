namespace AttendanceTracker.Contracts
{
    public class AttendanceReportResultDTO
    {
        public int WorkingDays { get; set; }

        public int CompletedDays { get; set; }

        public int OpenDays { get; set; }

        public TimeSpan TotalWorkingHours { get; set; }

        public TimeSpan AverageWorkingHours { get; set; }

        public DateTime? EarliestCheckIn { get; set; }

        public DateTime? LatestCheckOut { get; set; }

        public TimeSpan? LongestShift { get; set; }

        public DateOnly? LongestShiftDate { get; set; }

        public TimeSpan? ShortestShift { get; set; }

        public DateOnly? ShortestShiftDate { get; set; }

        public decimal? SalaryPerHour { get; set; }

        public decimal TotalSalary { get; set; }

        public List<AttendanceReportRowDTO> Records { get; set; } = [];
    }
}
