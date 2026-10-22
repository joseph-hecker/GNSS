using GNSS.Nmea;
using System.ComponentModel;

namespace GNSS.Test.Nmea;

[TestFixture]
public class NmeaEnumTests
{
    [Test, TestOf(nameof(NmeaType))]
    public void TestNmeaType([Values] NmeaType nmeaType)
    {
        static uint GetExpected(NmeaType nmeaType)
        {
            return nmeaType switch
            {
                NmeaType.Unknown => 0x00000000,
                NmeaType.GGA => 0x00000001,
                NmeaType.GLL => 0x00000002,
                NmeaType.GSA => 0x00000004,
                NmeaType.GST => 0x00000008,
                NmeaType.GSV => 0x00000010,
                NmeaType.RMC => 0x00000020,
                NmeaType.RME => 0x00000040,
                NmeaType.VTG => 0x00000080,
                NmeaType.ENAV => 0x00000100,
                _ => throw new InvalidEnumArgumentException(nameof(nmeaType), (int)nmeaType, typeof(NmeaType)),
            };
        }

        uint expected = GetExpected(nmeaType);
        Assert.That((uint)nmeaType, Is.EqualTo(expected));
    }

    [Test, TestOf(nameof(FixQuality))]
    public void TestFixQuality([Values] FixQuality fixQuality)
    {
        static int GetExpected(FixQuality quality)
        {
            return quality switch
            {
                FixQuality.Invalid => 0,
                FixQuality.GPS => 1,
                FixQuality.DGPS => 2,
                FixQuality.PPS => 3,
                FixQuality.RTKFixed => 4,
                FixQuality.RTKFloat => 5,
                FixQuality.Estimated => 6,
                FixQuality.Manual => 7,
                FixQuality.Simulation => 8,
                _ => throw new InvalidEnumArgumentException(nameof(quality), (int)quality, typeof(FixQuality)),
            };
        }

        int expected = GetExpected(fixQuality);
        Assert.That((int)fixQuality, Is.EqualTo(expected));
    }

    [Test, TestOf(nameof(FixQualityComparer))]
    public void TestFixQualityComparer()
    {
        FixQualityComparer comparer = new();
        Assert.Multiple(() =>
        {
            Assert.That(comparer.Compare(FixQuality.RTKFixed, FixQuality.RTKFloat), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.RTKFloat, FixQuality.DGPS), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.DGPS, FixQuality.GPS), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.GPS, FixQuality.PPS), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.GPS, FixQuality.Estimated), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.GPS, FixQuality.Manual), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.GPS, FixQuality.Simulation), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.PPS, FixQuality.Invalid), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.Estimated, FixQuality.Invalid), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.Manual, FixQuality.Invalid), Is.GreaterThan(0));
            Assert.That(comparer.Compare(FixQuality.Simulation, FixQuality.Invalid), Is.GreaterThan(0));
        });
    }

    [Test, TestOf(nameof(PositionMode))]
    public void TestPositionMode([Values] PositionMode positionMode)
    {
        static int GetExpected(PositionMode positionMode)
        {
            return positionMode switch
            {
                PositionMode.Invalid => 0,
                PositionMode.Autonomous => 1,
                PositionMode.Differential => 2,
                PositionMode.Estimated => 3,
                PositionMode.Manual => 4,
                PositionMode.Simulated => 5,
                _ => throw new InvalidEnumArgumentException(nameof(positionMode), (int)positionMode, typeof(PositionMode)),
            };
        }

        int expected = GetExpected(positionMode);
        Assert.That((int)positionMode, Is.EqualTo(expected));
    }

    [Test, TestOf(nameof(FixMode))]
    public void TestFixMode([Values] FixMode fixMode)
    {
        static int GetExpected(FixMode fixMode)
        {
            return fixMode switch
            {
                FixMode.None => 1,
                FixMode.TwoD => 2,
                FixMode.ThreeD => 3,
                _ => throw new InvalidEnumArgumentException(nameof(fixMode), (int)fixMode, typeof(FixMode)),
            };
        }

        int expected = GetExpected(fixMode);
        Assert.That((int)fixMode, Is.EqualTo(expected));
    }

    [Test, TestOf(nameof(EnavMode))]
    public void TestEnavMode([Values] EnavMode enavMode)
    {
        static int GetExpected(EnavMode enavMode)
        {
            return enavMode switch
            {
                EnavMode.GNSS => 0,
                EnavMode.Tilt => 1,
                EnavMode.InvisiblePole => 3,
                EnavMode.ExtensiblePoleTriggerUnpressed => 4,
                EnavMode.ExtensiblePoleTriggerPressed => 5,
                _ => throw new InvalidEnumArgumentException(nameof(enavMode), (int)enavMode, typeof(EnavMode)),
            };
        }

        int expected = GetExpected(enavMode);
        Assert.That((int)enavMode, Is.EqualTo(expected));
    }

    [Test, TestOf(nameof(SmartHandleStatus))]
    public void TestSmartHandleStatus([Values] SmartHandleStatus shStatus)
    {
        static int GetExpected(SmartHandleStatus shStatus)
        {
            return shStatus switch
            {
                SmartHandleStatus.None => 0,
                SmartHandleStatus.StandardHandle => 1,
                SmartHandleStatus.SmartHandleOff => 2,
                SmartHandleStatus.SmartHandleOn => 3,
                _ => throw new InvalidEnumArgumentException(nameof(shStatus), (int)shStatus, typeof(SmartHandleStatus)),
            };
        }

        int expected = GetExpected(shStatus);
        Assert.That((int)shStatus, Is.EqualTo(expected));
    }
}
