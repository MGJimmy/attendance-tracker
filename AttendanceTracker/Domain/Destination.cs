namespace AttendanceTracker.Domain;

public class Destination
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
