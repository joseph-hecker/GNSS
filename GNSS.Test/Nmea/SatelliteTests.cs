using GNSS.Nmea;

namespace GNSS.Test.Nmea;

public class SatelliteTests
{
    private const int REPEAT_COUNT = 10;

    private static Satellite RandomSatellite(int? seed = null)
    {
        Random random = (seed is null) ? new() : new(seed.Value);
        return new Satellite(random.Next(), random.Next(), random.Next(), random.Next());
    }

    [Test, TestOf(nameof(Satellite.Equals)), Repeat(REPEAT_COUNT)]
    public void TestEquals()
    {
        Satellite original = RandomSatellite();
        Satellite copy = original.Copy();
        Satellite different = RandomSatellite();
        Assert.Multiple(() =>
        {
            Assert.That(original, Is.EqualTo(copy));
            Assert.That(original, Is.Not.EqualTo(different));
        });
    }

    [Test, TestOf(nameof(Satellite.GetHashCode)), Repeat(REPEAT_COUNT)]
    public void TestGetHashCode()
    {
        Satellite original = RandomSatellite();
        Satellite copy = original.Copy();
        Satellite different = RandomSatellite();
        Assert.That(original.GetHashCode(), Is.EqualTo(copy.GetHashCode()));
        Assert.That(original.GetHashCode(), Is.Not.EqualTo(different.GetHashCode()));
    }
}
