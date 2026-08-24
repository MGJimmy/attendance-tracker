namespace AttendanceTracker.Infrastructure;

public static class EgyptTime
{
    public static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime UtcNow => DateTime.UtcNow;

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    public static DateOnly Today => DateOnly.FromDateTime(Now);

    public static DateTime ToUtc(DateTime egyptLocal)
    {
        if (egyptLocal.Kind == DateTimeKind.Utc)
            return egyptLocal;

        var unspecified = DateTime.SpecifyKind(egyptLocal, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, Zone);
    }

    public static DateOnly ToDate(DateTime utc)
    {
        var egypt = TimeZoneInfo.ConvertTimeFromUtc(EnsureUtc(utc), Zone);
        return DateOnly.FromDateTime(egypt);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone("Egypt", TimeSpan.FromHours(2), "Egypt", "Egypt");
    }
}
