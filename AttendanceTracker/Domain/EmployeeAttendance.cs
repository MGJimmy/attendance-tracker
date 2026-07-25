using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceTracker.Domain
{
    public class EmployeeAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;

        [Required]
        public DateOnly AttendanceDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [NotMapped]
        public TimeSpan? WorkingHours =>
            CheckInTime.HasValue && CheckOutTime.HasValue
                ? CheckOutTime - CheckInTime
                : null;
    }
}
