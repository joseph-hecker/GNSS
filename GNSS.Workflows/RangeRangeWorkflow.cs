using GNSS.Nmea;

namespace GNSS.Workflows;

public class RangeRangeWorkflow : GNSSWorkflow
{
    public RangeRangeWorkflow(GPS gps, Func<CancellationToken, Task> waitingForRequirements) : base(gps, waitingForRequirements) { }

    public override Task<GPSData> CollectVertexAsync()
    {
        throw new NotImplementedException();
    }

    private GPSData Intersection(GPSData left, double leftDistance, GPSData right, double rightDistance)
    {
        var p = Geometry.CircleCircleIntersection(left.Latitude, left.Longitude, leftDistance, right.Latitude, right.Longitude, rightDistance);
        GPSData data = new()
        {
            UTC = Max(left.UTC, right.UTC, datetime => datetime.Ticks),
            Latitude = default,
            Longitude = default,
            Altitude = (left.Altitude + right.Altitude) / 2,
            FixQuality = new FixQualityComparer().Max(left.FixQuality, right.FixQuality),
            PositionMode = default,
            FixMode = default,
            Status = default,
            NavStatus = default,
            HDOP = default,
            VDOP = default,
            PDOP = default,
            EstimatedAccuracy = default,
            Speed = default,
            Course = default,
            MagneticVariation = default,
            MagneticVariationDir = default,
            SatellitesInUse = default,
            SatellitesInView = default,
            Satellites = default,
            RMSRange = default,
            MajorStd = default,
            MinorStd = default,
            MajorAngle = default,
            LatStd = default,
            LonStd = default,
            AltStd = default,
            Age = default,
            StationID = default,
            EnavData = default,
        };

        return data;
    }

    private T Max<T>(T left, T right, Func<T, IComparable> getValue)
    {
        return getValue(left).CompareTo(getValue(right)) > 0 ? left : right;
    }

    private T Min<T>(T left, T right, Func<T, IComparable> getValue)
    {
        return getValue(left).CompareTo(getValue(right)) < 0 ? left : right;
    }
}
