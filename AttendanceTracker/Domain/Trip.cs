namespace AttendanceTracker.Domain;

public class Trip
{
    public int Id { get; set; }

    public int CourierId { get; set; }

    public Courier Courier { get; set; } = null!;

    public int DestinationId { get; set; }

    public Destination Destination { get; set; } = null!;

    public decimal Cost { get; set; }

    public DateOnly TripDate { get; set; }
}
