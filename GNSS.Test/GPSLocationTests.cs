using GNSS.Nmea;

namespace GNSS.Test;

public class GPSLocationTests
{
    private static GPSLocation RandomGPSLocation(int? seed = null)
    {
        Random random = (seed is null) ? new() : new(seed.Value);
        return new GPSLocation(
            Latitude: random.NextDouble() * 180 - 90,
            Longitude: random.NextDouble() * 360 - 180,
            Altitude: random.NextDouble() * 1500,
            Speed: random.NextDouble() * 50,
            Course: random.NextDouble() * 360,
            EstimatedAccuracy: random.NextDouble() * 10,
            FixQuality: random.GetItems(Enum.GetValues<FixQuality>(), 1)[0],
            UTC: new DateTime(random.Next(2020, 2026), random.Next(1, 13), random.Next(1, 28), random.Next(24), random.Next(60), random.Next(60)));
    }

    [Test, TestOf(nameof(GPSLocation.UpdateGPSData))]
    public void TestUpdateGPSData()
    {
        GPSLocation location = RandomGPSLocation();
        GPSData data = new();
        location.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.Latitude,          Is.EqualTo(location.Latitude));
            Assert.That(data.Longitude,         Is.EqualTo(location.Longitude));
            Assert.That(data.Altitude,          Is.EqualTo(location.Altitude));
            Assert.That(data.Speed,             Is.EqualTo(location.Speed));
            Assert.That(data.Course,            Is.EqualTo(location.Course));
            Assert.That(data.EstimatedAccuracy, Is.EqualTo(location.EstimatedAccuracy));
            Assert.That(data.FixQuality,        Is.EqualTo(location.FixQuality));
            Assert.That(data.UTC,               Is.EqualTo(location.UTC));
        });
    }
}
