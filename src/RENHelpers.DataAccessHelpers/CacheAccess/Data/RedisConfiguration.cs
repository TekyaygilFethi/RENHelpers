namespace RENHelpers.DataAccessHelpers.CacheAccess.Data;

public class RedisConfiguration
{
    public string Url { get; set; }
    public TimeConfiguration TimeConfiguration { get; set; }
    public int DatabaseId { get; set; }
}

public class TimeConfiguration
{
    public string AbsoluteExpirationInHours { get; set; }
    public string SlidingExpirationInMinutes { get; set; }
}