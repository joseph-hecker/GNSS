using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class RMC : NmeaSentence, ILocation
{
    public TimeOnly Time { get; }
    public string Status { get; }
    public string Lat { get; }
    public string LatDir { get; }
    public string Lon { get; }
    public string LonDir { get; }
    public double Speed { get; }
    public double Course { get; }
    public DateOnly Date { get; }
    public double MagneticVariation { get; }
    public string MagneticVariationDir { get; }
    public PositionMode PositionMode { get; }
    public string NavStatus { get; }

    public RMC(string message) : base(message)
    {
        try
        {
            Time = ParseTime(_parts[1]);
            Status = _parts[2];
            Lat = _parts[3];
            LatDir = _parts[4];
            Lon = _parts[5];
            LonDir = _parts[6];
            Speed = double.Parse(_parts[7]);
            Course = double.Parse(_parts[8]);
            Date = ParseDate(_parts[9]);
            MagneticVariation = double.Parse(_parts[10]);
            MagneticVariationDir = _parts[11];
            PositionMode = ParsePositionMode(_parts[12]);
            NavStatus = _parts[13];
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing RMC message.", ex, message);
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
        data.UTCDate = Date;
        data.Latitude = Latitude();
        data.Longitude = Longitude();
        data.PositionMode = PositionMode;
        data.Status = Status;
        data.NavStatus = NavStatus;
        data.Speed = Speed;
        data.Course = Course;
        data.MagneticVariation = MagneticVariation;
        data.MagneticVariationDir = MagneticVariationDir;
    }
}
