using GNSS.Nmea;
using System.Collections;
using System.Text;

namespace GNSS;

public class GPSData : IEquatable<GPSData>
{
    internal DateOnly UTCDate { get; set; } = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    internal TimeOnly UTCTime { get; set; } = new(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
    public DateTime UTC => new(UTCDate, UTCTime);
    public double Latitude { get; internal set; }
    public double Longitude { get; internal set; }
    public double Altitude { get; internal set; }
    public FixQuality FixQuality { get; internal set; }
    public PositionMode PositionMode { get; internal set; }
    public FixMode FixMode { get; internal set; }
    public string Status { get; internal set; } = string.Empty;
    public string NavStatus { get; internal set; } = string.Empty;
    public double HDOP { get; internal set; }
    public double VDOP { get; internal set; }
    public double PDOP { get; internal set; }
    public double EstimatedAccuracy { get; internal set; }
    public double Speed { get; internal set; }
    public double Course { get; internal set; }
    public double MagneticVariation { get; internal set; }
    public string MagneticVariationDir { get; internal set; } = string.Empty;
    public int SatellitesInUse { get; internal set; }
    public int SatellitesInView { get; internal set; }
    public List<Satellite> Satellites { get; internal set; } = [];
    public double RMSRange { get; internal set; }
    public double MajorStd { get; internal set; }
    public double MinorStd { get; internal set; }
    public double MajorAngle { get; internal set; }
    public double LatStd { get; internal set; }
    public double LonStd { get; internal set; }
    public double AltStd { get; internal set; }
    public int? Age { get; internal set; }
    public int? StationID { get; internal set; }
    public ENAV? EnavData { get; internal set; }

    public GPSData Copy()
    {
        return new()
        {
            UTCDate = UTCDate,
            UTCTime = UTCTime,
            Latitude = Latitude,
            Longitude = Longitude,
            Altitude = Altitude,
            FixQuality = FixQuality,
            PositionMode = PositionMode,
            FixMode = FixMode,
            Status = Status,
            NavStatus = NavStatus,
            HDOP = HDOP,
            VDOP = VDOP,
            PDOP = PDOP,
            EstimatedAccuracy = EstimatedAccuracy,
            Speed = Speed,
            Course = Course,
            MagneticVariation = MagneticVariation,
            MagneticVariationDir = MagneticVariationDir,
            SatellitesInUse = SatellitesInUse,
            SatellitesInView = SatellitesInView,
            Satellites = [.. Satellites.Select(s => new Satellite(s.PRN, s.Elevation, s.Azimuth, s.SNR))],
            RMSRange = RMSRange,
            MajorStd = MajorStd,
            MinorStd = MinorStd,
            MajorAngle = MajorAngle,
            LatStd = LatStd,
            LonStd = LonStd,
            AltStd = AltStd,
            Age = Age,
            StationID = StationID,
            EnavData = EnavData,
        };
    }

    public bool Equals(GPSData? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return UTCDate == other.UTCDate &&
               UTCTime == other.UTCTime &&
               Latitude == other.Latitude &&
               Longitude == other.Longitude &&
               Altitude == other.Altitude &&
               FixQuality == other.FixQuality &&
               PositionMode == other.PositionMode &&
               FixMode == other.FixMode &&
               Status == other.Status &&
               NavStatus == other.NavStatus &&
               HDOP == other.HDOP &&
               VDOP == other.VDOP &&
               PDOP == other.PDOP &&
               EstimatedAccuracy == other.EstimatedAccuracy &&
               Speed == other.Speed &&
               Course == other.Course &&
               MagneticVariation == other.MagneticVariation &&
               MagneticVariationDir == other.MagneticVariationDir &&
               SatellitesInUse == other.SatellitesInUse &&
               SatellitesInView == other.SatellitesInView &&
               Satellites.SequenceEqual(other.Satellites) &&
               RMSRange == other.RMSRange &&
               MajorStd == other.MajorStd &&
               MinorStd == other.MinorStd &&
               MajorAngle == other.MajorAngle &&
               LatStd == other.LatStd &&
               LonStd == other.LonStd &&
               AltStd == other.AltStd &&
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
