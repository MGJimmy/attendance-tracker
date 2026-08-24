namespace AttendanceTracker.Contracts;

public class AddDestinationDTO
{
    public string Name { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public decimal Cost { get; set; }
}

public class EditDestinationDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public decimal Cost { get; set; }
}
