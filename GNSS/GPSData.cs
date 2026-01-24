using GNSS.Nmea;
using System.Net.Http.Headers;

namespace GNSS;

public class GPSData : IEquatable<GPSData>
{
    internal DateOnly UTCDate { get; set; }
    internal TimeOnly UTCTime { get; set; }
    public DateTime UTC => new(UTCDate, UTCTime);
    public double Latitude { get; internal set; }
    public double Longitude { get; internal set; }
    public double Altitude { get; internal set; }
    public Fix Fix { get; internal set; }
    public FixMode FixMode { get; internal set; }
    public double HDOP { get; internal set; }
    public double VDOP { get; internal set; }
    public double PDOP { get; internal set; }
    public double EstimatedAccuracy { get; internal set; }
    public int SatellitesInUse { get; internal set; }
    public int SatellitesInView { get; internal set; }
    public IEnumerable<Satellite> Satellites { get; internal set; } = [];
    public int? Age { get; internal set; }
    public int? StationID { get; internal set; }
    public ENAV? EnavData { get; internal set; }

    internal GPSData Copy()
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
            Satellites = Satellites,
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
               (EnavData?.Equals(other.EnavData) ?? false);
    }
}
