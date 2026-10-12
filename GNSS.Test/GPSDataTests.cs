using GNSS.Nmea;

namespace GNSS.Test;

//[TestFixture, TestOf(typeof(GPSData))]
//public class GPSDataTests
//{
//    private const int REPEAT_COUNT = 10;
//
//    private static GPSData RandomGPSData(int? seed = null)
//    {
//        Random random = (seed is null) ? new() : new(seed.Value);
//        GPSData data = new()
//        {
//            Latitude = random.NextDouble() * 180 - 90,
//            Longitude = random.NextDouble() * 360 - 180,
//            Altitude = random.NextDouble() * 1500,
//            FixQuality = (FixQuality)random.Next(Enum.GetValues<FixQuality>().Length),
//            FixMode = (FixMode)random.Next(Enum.GetValues<FixMode>().Length),
//            HDOP = random.NextDouble(),
//            VDOP = random.NextDouble(),
//            PDOP = random.NextDouble(),
//            EstimatedAccuracy = random.NextDouble() * 10,
//            Age = random.Next(10),
//            StationID = random.Next(255),
//            EnavData = null,
//            SatellitesInUse = random.Next(15)
//        };
//        data.SatellitesInView = random.Next(data.SatellitesInUse, data.SatellitesInUse + 10);
//        data.Satellites = Enumerable.Range(0, data.SatellitesInView).Select(_ => new Satellite(random.Next(200), random.Next(90), random.Next(360), random.Next(10, 40))).ToList();
//    
//        return data;
//    }
//    
//    [Test, TestOf(nameof(GPSData.Copy)), Repeat(REPEAT_COUNT)]
//    public void TestCopy()
//    {
//        GPSData original = RandomGPSData();
//        GPSData copy = original.Copy();
//        Assert.That(copy, Is.Not.SameAs(original));
//        Assert.That(copy, Is.Not.EqualTo(new GPSData()));
//        Assert.Multiple(() =>
//        {
//            foreach (var prop in typeof(GPSData).GetProperties().Where(p => p.CanRead))
//            {
//                var originalValue = prop.GetValue(original);
//                var copiedValue = prop.GetValue(copy);
//                Assert.That(copiedValue, Is.EqualTo(originalValue), $"Failed on property: {prop.Name}");
//            }
//        });
//    }
//    
//    [Test, TestOf(nameof(GPSData.Equals)), Repeat(REPEAT_COUNT)]
//    public void TestEquals()
//    {
//        GPSData original = RandomGPSData();
//        GPSData copy = original.Copy();
//        GPSData different = RandomGPSData();
//        Assert.Multiple(() =>
//        {
//            Assert.That(original, Is.Not.Null);
//            Assert.That(original, Is.EqualTo(copy));
//            Assert.That(original, Is.Not.EqualTo(different));
//            Assert.That(original, Is.EqualTo((object)copy));
//            Assert.That(original, Is.Not.EqualTo((object)different));
//        });
//    }
//    
//    [Test, TestOf(nameof(GPSData.GetHashCode)), Repeat(REPEAT_COUNT)]
//    public void TestGetHashCode()
//    {
//        GPSData original = RandomGPSData();
//        GPSData copy = original.Copy();
//        GPSData different = RandomGPSData();
//        Assert.That(original.GetHashCode(), Is.EqualTo(copy.GetHashCode()));
//        Assert.That(original.GetHashCode(), Is.Not.EqualTo(different.GetHashCode()));
//    }
//    
//    [Test, TestOf(nameof(GPSData.ToString)), Repeat(REPEAT_COUNT)]
//    public void TestToString()
//    {
//        GPSData original = RandomGPSData();
//        GPSData copy = original.Copy();
//        GPSData different = RandomGPSData();
//        Assert.That(original.ToString(), Is.EqualTo(copy.ToString()));
//        Assert.That(original.ToString(), Is.Not.EqualTo(different.ToString()));
//    }
//}
