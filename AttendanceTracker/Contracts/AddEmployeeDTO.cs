namespace AttendanceTracker.Contracts
{
    public class AddEmployeeDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal SalaryPerHour { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
