using GNSS.Nmea;

namespace GNSS.Test.Nmea;

public class NmeaEnumTests
{
    [Test, TestOf(nameof(NmeaType))]
    public void TestNmeaType([Values] NmeaType nmeaType)
    {
        uint? GetExpected(NmeaType nmeaType)
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
                _ => null,
            };
        }

        uint? expected = GetExpected(nmeaType);
        Assert.That((uint)nmeaType, Is.EqualTo(expected), $"", nmeaType.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
    }

    [Test, TestOf(nameof(FixQuality))]
    public void TestFixQuality([Values] FixQuality quality)
    {
        int? GetExpected(FixQuality quality)
        {
            return quality switch
            {
                FixQuality.Invalid => 0,
                FixQuality.GPS => 1,
                FixQuality.DGPS => 2,
                FixQuality.PPS => 3,
                FixQuality.RTKFix => 4,
                FixQuality.RTKFloat => 5,
                FixQuality.Estimated => 6,
                FixQuality.Manual => 7,
                FixQuality.Simulation => 8,
                _ => null,
            };
        }

        int? expected = GetExpected(quality);
        Assert.That((int)quality, Is.EqualTo(expected), $"", quality.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
    }

    [Test, TestOf(nameof(PositionMode))]
    public void TestPositionMode([Values] PositionMode positionMode)
    {
        int? GetExpected(PositionMode positionMode)
        {
            return positionMode switch
            {
                PositionMode.Invalid => 0,
                PositionMode.Autonomous => 1,
                PositionMode.Differential => 2,
                PositionMode.Estimated => 3,
                PositionMode.Manual => 4,
                PositionMode.Simulated => 5,
                _ => null,
            };
        }

        int? expected = GetExpected(positionMode);
        Assert.That((int)positionMode, Is.EqualTo(expected), $"", positionMode.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
    }

    [Test, TestOf(nameof(FixMode))]
    public void TestFixMode([Values] FixMode fixMode)
    {
        int? GetExpected(FixMode fixMode)
        {
            return fixMode switch
            {
                FixMode.None => 1,
                FixMode.TwoD => 2,
                FixMode.ThreeD => 3,
                _ => null,
            };
        }

        int? expected = GetExpected(fixMode);
        Assert.That((int)fixMode, Is.EqualTo(expected), $"", fixMode.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
    }

    [Test, TestOf(nameof(EnavMode))]
    public void TestEnavMode([Values] EnavMode enavMode)
    {
        int? GetExpected(EnavMode enavMode)
        {
            return enavMode switch
            {
                EnavMode.GNSS => 0,
                EnavMode.Tilt => 1,
                EnavMode.InvisiblePole => 3,
                EnavMode.ExtensiblePoleTriggerUnpressed => 4,
                EnavMode.ExtensiblePoleTriggerPressed => 5,
                _ => null,
            };
        }

        int? expected = GetExpected(enavMode);
        Assert.That((int)enavMode, Is.EqualTo(expected), $"", enavMode.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
    }

    [Test, TestOf(nameof(SmartHandleStatus))]
    public void TestSmartHandleStatus([Values] SmartHandleStatus shStatus)
    {
        int? GetExpected(SmartHandleStatus shStatus)
        {
            return shStatus switch
            {
                SmartHandleStatus.None => 0,
                SmartHandleStatus.StandardHandle => 1,
                SmartHandleStatus.SmartHandleOff => 2,
                SmartHandleStatus.SmartHandleOn => 3,
                _ => null,
            };
        }

        int? expected = GetExpected(shStatus);
        Assert.That((int)shStatus, Is.EqualTo(expected), $"", shStatus.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
    }
}
