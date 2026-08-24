namespace AttendanceTracker.Domain
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal SalaryPerHour { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<EmployeeAttendance> Attendances { get; set; } = new List<EmployeeAttendance>();
    }
}
