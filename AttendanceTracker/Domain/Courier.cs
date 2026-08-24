namespace AttendanceTracker.Domain;

public class Courier
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
