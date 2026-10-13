using GNSS.Nmea;
using GNSS.Test.Nmea;

namespace GNSS.Test;

public class GPSDataTests
{
    private const int REPEAT_COUNT = 10;

    private static GPSData RandomGPSData(int? seed = null)
    {
        Random random = (seed is null) ? new() : new(seed.Value);
        GPSData data = new()
        {
            UTC = new DateTime(random.Next(2020, 2026), random.Next(1, 13), random.Next(1, 28), random.Next(24), random.Next(60), random.Next(60)),
            Latitude = random.NextDouble() * 180 - 90,
            Longitude = random.NextDouble() * 360 - 180,
            Altitude = random.NextDouble() * 1500,
            FixQuality = random.GetItems(Enum.GetValues<FixQuality>(), 1)[0],
            PositionMode = random.GetItems(Enum.GetValues<PositionMode>(), 1)[0],
            FixMode = random.GetItems(Enum.GetValues<FixMode>(), 1)[0],
            Status = random.GetItems(["A", "V"], 1)[0],
            NavStatus = random.GetItems(["S", "U", "C", "V"], 1)[0],
            HDOP = random.NextDouble(),
            VDOP = random.NextDouble(),
            PDOP = random.NextDouble(),
            EstimatedAccuracy = random.NextDouble() * 10,
            Speed = random.NextDouble() * 50,
            Course = random.NextDouble() * 360,
            MagneticVariation = random.NextDouble() * 20,
            MagneticVariationDir = random.GetItems(["E", "W"], 1)[0],
            SatellitesInUse = random.Next(15),
            RMSRange = random.NextDouble(),
            MajorStd = random.NextDouble(),
            MinorStd = random.NextDouble(),
            MajorAngle = random.NextDouble() * 360,
            LatStd = random.NextDouble(),
            LonStd = random.NextDouble(),
            AltStd = random.NextDouble(),
            Age = random.Next(10),
            StationID = random.Next(255),
            EnavData = new ENAV(random.GetItems([.. NmeaSource.GetENAVMessages()], 1)[0]),
        };
        data.SatellitesInView = random.Next(data.SatellitesInUse, data.SatellitesInUse + 10);
        data.Satellites = [.. Enumerable.Range(0, data.SatellitesInView).Select(_ => new Satellite(random.Next(200), random.Next(90), random.Next(360), random.Next(10, 40)))];
    
        return data;
    }
    
    [Test, TestOf(nameof(GPSData.Equals)), Repeat(REPEAT_COUNT)]
    public void TestEquals()
    {
        GPSData original = RandomGPSData();
        GPSData copy = original.Copy();
        GPSData different = RandomGPSData();
        Assert.Multiple(() =>
        {
            Assert.That(original, Is.EqualTo(copy));
            Assert.That(original, Is.Not.EqualTo(different));
        });
    }
    
    [Test, TestOf(nameof(GPSData.GetHashCode)), Repeat(REPEAT_COUNT)]
    public void TestGetHashCode()
    {
        GPSData original = RandomGPSData();
        GPSData copy = original.Copy();
        GPSData different = RandomGPSData();
        Assert.That(original.GetHashCode(), Is.EqualTo(copy.GetHashCode()));
        Assert.That(original.GetHashCode(), Is.Not.EqualTo(different.GetHashCode()));
    }
}
