using GNSS.Nmea;
using System.Net.Http.Headers;

namespace GNSS;

public class GPSData
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

    internal GPSData Copy()
    {
        return new()
        {
            UTC = UTC,
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
            StationID = StationID
        };
    }
}
