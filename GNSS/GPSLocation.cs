using GNSS.Nmea;

namespace GNSS;

/// <summary>
/// A sparse position patch from a non-NMEA source.
/// </summary>
public readonly record struct GPSLocation(
    double Latitude,
    double Longitude,
    double? Altitude = null,
    double? Speed = null,
    double? Course = null,
    double? EstimatedAccuracy = null,
    FixQuality? FixQuality = null,
    DateTime? UTC = null) : ILocationInfo
{
    public void UpdateGPSData(GPSData data)
    {
        data.Latitude = Latitude;
        data.Longitude = Longitude;
        if (Altitude.HasValue)
            data.Altitude = Altitude.Value;
        if (Speed.HasValue)
            data.Speed = Speed.Value;
        if (Course.HasValue)
            data.Course = Course.Value;
        if (EstimatedAccuracy.HasValue)
            data.EstimatedAccuracy = EstimatedAccuracy.Value;
        if (FixQuality.HasValue)
            data.FixQuality = FixQuality.Value;
        if (UTC is DateTime utc)
        {
            data.UTCDate = DateOnly.FromDateTime(utc);
            data.UTCTime = TimeOnly.FromDateTime(utc);
        }
    }
}
