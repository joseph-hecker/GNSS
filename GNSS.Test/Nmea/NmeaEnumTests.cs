using GNSS.Nmea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    [Test, TestOf(nameof(Fix))]
    public void TestFix([Values] Fix fix)
    {
        int? GetExpected(Fix fix)
        {
            return fix switch
            {
                Fix.Invalid => 0,
                Fix.GPS => 1,
                Fix.DGPS => 2,
                Fix.RTKFix => 4,
                Fix.RTKFloat => 5,
                _ => null,
            };
        }

        int? expected = GetExpected(fix);
        Assert.That((int)fix, Is.EqualTo(expected), $"", fix.ToString(), $"Is.EqualTo({expected?.ToString() ?? "null"})");
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
