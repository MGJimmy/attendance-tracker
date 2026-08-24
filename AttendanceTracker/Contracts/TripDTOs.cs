namespace AttendanceTracker.Contracts;

public class AddTripDTO
{
    public int CourierId { get; set; }

    public int DestinationId { get; set; }

    public decimal? Cost { get; set; }

    public DateOnly TripDate { get; set; }
}

public class EditTripDTO
{
    public int Id { get; set; }

    public int CourierId { get; set; }

    public int DestinationId { get; set; }

    public decimal Cost { get; set; }

    public DateOnly TripDate { get; set; }
}

public class TripListDTO
{
    public int Id { get; set; }

    public int CourierId { get; set; }

    public string CourierName { get; set; } = string.Empty;

    public int DestinationId { get; set; }

    public string DestinationName { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public DateOnly TripDate { get; set; }
}
