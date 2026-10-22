using GNSS.Nmea;

namespace GNSS;

public class GPSData : IEquatable<GPSData>
{
    internal DateOnly UTCDate { get; set; } = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    internal TimeOnly UTCTime { get; set; } = new(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
    public DateTime UTC
    {
        get => new(UTCDate, UTCTime);
        set
        {
            UTCDate = DateOnly.FromDateTime(value);
            UTCTime = TimeOnly.FromDateTime(value);
        }
    }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public FixQuality FixQuality { get; set; }
    public PositionMode PositionMode { get; set; }
    public FixMode FixMode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string NavStatus { get; set; } = string.Empty;
    public double HDOP { get; set; }
    public double VDOP { get; set; }
    public double PDOP { get; set; }
    public double EstimatedAccuracy { get; set; }
    public double Speed { get; set; }
    public double Course { get; set; }
    public double MagneticVariation { get; set; }
    public string MagneticVariationDir { get; set; } = string.Empty;
    public int SatellitesInUse { get; set; }
    public int SatellitesInView { get; set; }
    public List<Satellite> Satellites { get; set; } = [];
    public double RMSRange { get; set; }
    public double MajorStd { get; set; }
    public double MinorStd { get; set; }
    public double MajorAngle { get; set; }
    public double LatStd { get; set; }
    public double LonStd { get; set; }
    public double AltStd { get; set; }
    public double? Age { get; set; }
    public int? StationID { get; set; }
    public ENAV? EnavData { get; set; }

    public GPSData() { }

    public GPSData(GPSLocation location)
    {
        Latitude = Latitude;
        Longitude = Longitude;
        if (location.Altitude.HasValue)
            Altitude = location.Altitude.Value;
        if (location.Speed.HasValue)
            Speed = location.Speed.Value;
        if (location.Course.HasValue)
            Course = location.Course.Value;
        if (location.EstimatedAccuracy.HasValue)
            EstimatedAccuracy = location.EstimatedAccuracy.Value;
        if (location.FixQuality.HasValue)
            FixQuality = location.FixQuality.Value;
        if (location.UTC is DateTime utc)
        {
            UTCDate = DateOnly.FromDateTime(utc);
            UTCTime = TimeOnly.FromDateTime(utc);
        }
    }

    public GPSData Copy() => new()
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
        Satellites = [.. Satellites.Select(sat => sat.Copy())],
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
        hash.Add(UTCDate);
        hash.Add(UTCTime);
        hash.Add(Latitude);
        hash.Add(Longitude);
        hash.Add(Altitude);
        hash.Add(FixQuality);
        hash.Add(PositionMode);
        hash.Add(FixMode);
        hash.Add(Status);
        hash.Add(NavStatus);
        hash.Add(HDOP);
        hash.Add(VDOP);
        hash.Add(PDOP);
        hash.Add(EstimatedAccuracy);
        hash.Add(Speed);
        hash.Add(Course);
        hash.Add(MagneticVariation);
        hash.Add(MagneticVariationDir);
        hash.Add(SatellitesInUse);
        hash.Add(SatellitesInView);
        Satellites.ForEach(hash.Add);
        hash.Add(RMSRange);
        hash.Add(MajorStd);
        hash.Add(MinorStd);
        hash.Add(MajorAngle);
        hash.Add(LatStd);
        hash.Add(LonStd);
        hash.Add(AltStd);
        hash.Add(Age);
        hash.Add(StationID);
        hash.Add(EnavData);

        return hash.ToHashCode();
    }
}
