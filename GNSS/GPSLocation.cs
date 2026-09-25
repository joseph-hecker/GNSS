namespace GNSS;

/// <summary>
/// A single position update from a non-NMEA source (device GPS, map center, etc.).
/// </summary>
public readonly record struct GPSLocation(
    double Latitude,
    double Longitude,
    double? Altitude = null,
    double? EstimatedAccuracy = null)
{
    public void UpdateGPSData(GPSData data)
    {
        data.Latitude = Latitude;
        data.Longitude = Longitude;
        if (Altitude.HasValue) data.Altitude = Altitude.Value;
        if (EstimatedAccuracy.HasValue) data.EstimatedAccuracy = EstimatedAccuracy.Value;
    }
}
