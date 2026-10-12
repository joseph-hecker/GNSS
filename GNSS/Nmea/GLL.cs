using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class GLL : NmeaSentence, ILocation
{
    public string Lat { get; }
    public string LatDir { get; }
    public string Lon { get; }
    public string LonDir { get; }
    public TimeOnly Time { get; }
    public string Status { get; }
    public PositionMode PositionMode { get; }

    public GLL(string message) : base(message)
    {
        try
        {
            Lat = _parts[1];
            LatDir = _parts[2];
            Lon = _parts[3];
            LonDir = _parts[4];
            Time = ParseTime(_parts[5]);
            Status = _parts[6];
            PositionMode = ParsePositionMode(_parts[7]);
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing GLL message.", ex, message);
        }
    }

    public double Latitude()
    {
        int sign = LatDir == "N" ? 1 : -1;
        int degrees = int.Parse(Lat[..2]);
        double minutes = double.Parse(Lat[2..]) / 60;

        return sign * (degrees + minutes);
    }

    public double Longitude()
    {
        int sign = LonDir == "E" ? 1 : -1;
        int degrees = int.Parse(Lon[..3]);
        double minutes = double.Parse(Lon[3..]) / 60;

        return sign * (degrees + minutes);
    }

    public override void UpdateGPSData(GPSData data)
    {
        data.UTCTime = Time;
        data.Latitude = Latitude();
        data.Longitude = Longitude();
        data.PositionMode = PositionMode;
        data.Status = Status;
    }
}
