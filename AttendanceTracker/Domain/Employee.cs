namespace AttendanceTracker.Domain
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<EmployeeAttendance> Attendances { get; set; } = new List<EmployeeAttendance>();
    }
}
