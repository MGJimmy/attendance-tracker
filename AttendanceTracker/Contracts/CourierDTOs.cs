namespace AttendanceTracker.Contracts;

public class AddCourierDTO
{
    public string Name { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;
}

public class EditCourierDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;
}

public class CourierSummaryDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int TotalTrips { get; set; }

    public decimal TotalPay { get; set; }

    public List<CourierTripRowDTO> Trips { get; set; } = [];
}

public class CourierTripRowDTO
{
    public int Id { get; set; }

    public string DestinationName { get; set; } = string.Empty;

    public DateOnly TripDate { get; set; }

    public decimal Cost { get; set; }
}
