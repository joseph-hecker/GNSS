using GNSS.Nmea;
using System.Collections;
using System.Text;

namespace GNSS;

public class GPSData : IEquatable<GPSData>
{
    internal DateOnly UTCDate { get; set; } = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    internal TimeOnly UTCTime { get; set; } = new(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
    public DateTime UTC => new(UTCDate, UTCTime);
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public Fix Fix { get; set; }
    public FixMode FixMode { get; set; }
    public double HDOP { get; set; }
    public double VDOP { get; set; }
    public double PDOP { get; set; }
    public double EstimatedAccuracy { get; set; }
    public int SatellitesInUse { get; set; }
    public int SatellitesInView { get; set; }
    public IEnumerable<Satellite> Satellites { get; set; } = [];
    public int? Age { get; set; }
    public int? StationID { get; set; }
    public ENAV? EnavData { get; set; }

    public GPSData Copy()
    {
        return new()
        {
            UTCDate = UTCDate,
            UTCTime = UTCTime,
            Latitude = Latitude,
            Longitude = Longitude,
            Altitude = Altitude,
            Fix = Fix,
            FixMode = FixMode,
            HDOP = HDOP,
            VDOP = VDOP,
            PDOP = PDOP,
            EstimatedAccuracy = EstimatedAccuracy,
            SatellitesInUse = SatellitesInUse,
            SatellitesInView = SatellitesInView,
            Satellites = Satellites.Select(s => new Satellite(s.PRN, s.Elevation, s.Azimuth, s.SNR)),
            Age = Age,
            StationID = StationID,
            EnavData = EnavData,
        };
    }

    public bool Equals(GPSData? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return UTCDate == other.UTCDate &&
               UTCTime == other.UTCTime &&
               Latitude == other.Latitude &&
               Longitude == other.Longitude &&
               Altitude == other.Altitude &&
               Fix == other.Fix &&
               FixMode == other.FixMode &&
               HDOP == other.HDOP &&
               VDOP == other.VDOP &&
               PDOP == other.PDOP &&
               EstimatedAccuracy == other.EstimatedAccuracy &&
               SatellitesInUse == other.SatellitesInUse &&
               SatellitesInView == other.SatellitesInView &&
               Satellites.SequenceEqual(other.Satellites) &&
               Age == other.Age &&
               StationID == other.StationID &&
               (EnavData?.Equals(other.EnavData) ?? other.EnavData is null);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as GPSData);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        foreach (var prop in typeof(GPSData).GetProperties().Where(p => p.CanRead))
        {
            var value = prop.GetValue(this);
            if (value is not null)
            {
                hash.Add(value);
            }
        }

        return hash.ToHashCode();
    }

    public override string ToString()
    {
        int indentLevel = 0;
        string EnumerableToString(IEnumerable? enumerable)
        {
            if (enumerable is null)
            {
                return "null";
            }

            StringBuilder sb = new("[\n");
            indentLevel++;
            foreach (var e in enumerable)
            {
                sb.Append('\t', indentLevel);
                sb.AppendLine(e?.ToString());
            }
            indentLevel--;
            sb.Append('\t', indentLevel);
            sb.Append(']');

            return sb.ToString();
        }

        StringBuilder sb = new("{\n");
        indentLevel++;
        foreach (var prop in typeof(GPSData).GetProperties().Where(p => p.CanRead))
        {
            var value = prop.GetValue(this);
            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
            {
                sb.Append('\t', indentLevel);
                sb.AppendLine($"{prop.Name}: {EnumerableToString(value as IEnumerable)},");
            }
            else
            {
                sb.Append('\t', indentLevel);
                sb.AppendLine($"{prop.Name}: {value},");
            }
        }
        indentLevel--;
        sb.Append('\t', indentLevel);
        sb.Append('}');

        return sb.ToString();
    }
}
