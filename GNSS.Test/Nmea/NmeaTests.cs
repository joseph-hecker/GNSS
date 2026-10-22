using GNSS.Nmea;
using System.ComponentModel;

namespace GNSS.Test.Nmea;

public class NmeaTests
{
    private static int NmeaIndex { get; set; }

    private static string FormatErrorMessage()
    {
        return $"Error validating nmea message on field {NmeaIndex}.";
    }

    private static string GetPositionModeCode(PositionMode mode)
    {
        return mode switch
        {
            PositionMode.Autonomous => "A",
            PositionMode.Differential => "D",
            PositionMode.Estimated => "E",
            PositionMode.Manual => "M",
            PositionMode.Simulated => "S",
            PositionMode.Invalid => "N",
            _ => throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(PositionMode)),
        };
    }

    [SetUp]
    public void SetUp()
    {
        NmeaIndex = 0;
    }

    [Test, TestOf(nameof(GGA))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetGGAMessages))]
    public void TestGGA(string nmea)
    {
        GGA gga = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(gga.ID,                                Is.EqualTo(NmeaType.GGA), FormatErrorMessage);
            Assert.That(gga.Time.ToString("HHmmss.ff"),        Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Lat,                               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.LatDir,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Lon,                               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.LonDir,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.FixQuality.ToString("d"),          Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.SatsUsed.ToString("00"),           Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Hdop.ToString(),                   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Alt.ToString("####.000"),          Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.AltUnits,                          Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Undulation.ToString(),             Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.UndulationUnits,                   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Age?.ToString("##.0") ?? "",       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.StationID?.ToString("0000") ?? "", Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        gga.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC.TimeOfDay,   Is.EqualTo(gga.Time.ToTimeSpan()));
            Assert.That(data.Latitude,        Is.EqualTo(gga.Latitude()));
            Assert.That(data.Longitude,       Is.EqualTo(gga.Longitude()));
            Assert.That(data.Altitude,        Is.EqualTo(gga.Alt));
            Assert.That(data.FixQuality,      Is.EqualTo(gga.FixQuality));
            Assert.That(data.HDOP,            Is.EqualTo(gga.Hdop));
            Assert.That(data.SatellitesInUse, Is.EqualTo(gga.SatsUsed));
            Assert.That(data.Age,             Is.EqualTo(gga.Age));
            Assert.That(data.StationID,       Is.EqualTo(gga.StationID));
        });

        Assert.That(gga.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gga));
    }

    [Test, TestOf(nameof(GLL))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetGLLMessages))]
    public void TestGLL(string nmea)
    {
        GLL gll = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(gll.ID,                                Is.EqualTo(NmeaType.GLL), FormatErrorMessage);
            Assert.That(gll.Lat,                               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gll.LatDir,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gll.Lon,                               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gll.LonDir,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gll.Time.ToString("HHmmss.ff"),        Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gll.Status,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(GetPositionModeCode(gll.PositionMode), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        gll.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC.TimeOfDay, Is.EqualTo(gll.Time.ToTimeSpan()));
            Assert.That(data.Latitude,      Is.EqualTo(gll.Latitude()));
            Assert.That(data.Longitude,     Is.EqualTo(gll.Longitude()));
            Assert.That(data.PositionMode,  Is.EqualTo(gll.PositionMode));
            Assert.That(data.Status,        Is.EqualTo(gll.Status));
        });

        Assert.That(gll.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gll));
    }

    [Test, TestOf(nameof(GSA))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetGSAMessages))]
    public void TestGSA(string nmea)
    {
        GSA gsa = new(nmea);
        Assert.Multiple(() => 
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(gsa.ID,                    Is.EqualTo(NmeaType.GSA), FormatErrorMessage);
            Assert.That(gsa.SelectionMode,         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gsa.FixMode.ToString("d"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            for (int i = 0; i < expected.Length - 7; i++)
            {
                string prn = (gsa.PRNs.Count > i) ? gsa.PRNs[i].ToString("00") : "";
                Assert.That(prn,                   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            }
            Assert.That(gsa.VDOP.ToString(),       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gsa.HDOP.ToString(),       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gsa.PDOP.ToString(),       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gsa.GNSSID.ToString(),     Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        gsa.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.FixMode, Is.EqualTo(gsa.FixMode));
            Assert.That(data.PDOP,  Is.EqualTo(gsa.PDOP));
            Assert.That(data.HDOP,  Is.EqualTo(gsa.HDOP));
            Assert.That(data.VDOP,  Is.EqualTo(gsa.VDOP));
        });

        Assert.That(gsa.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gsa));
    }

    [Test, TestOf(nameof(GST))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetGSTMessages))]
    public void TestGST(string nmea)
    {
        GST gst = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(gst.ID,                         Is.EqualTo(NmeaType.GST), FormatErrorMessage);
            Assert.That(gst.Time.ToString("HHmmss.ff"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.RMSRange.ToString("0.000"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.MajorStd.ToString("0.000"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.MinorStd.ToString(),        Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.MajorAngle.ToString(),      Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.LatStd.ToString("0.000"),   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.LonStd.ToString(),          Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gst.AltStd.ToString(),          Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        gst.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC.TimeOfDay,      Is.EqualTo(gst.Time.ToTimeSpan()));
            Assert.That(data.RMSRange,           Is.EqualTo(gst.RMSRange));
            Assert.That(data.MajorStd,           Is.EqualTo(gst.MajorStd));
            Assert.That(data.MinorStd,           Is.EqualTo(gst.MinorStd));
            Assert.That(data.MajorAngle,         Is.EqualTo(gst.MajorAngle));
            Assert.That(data.LatStd,             Is.EqualTo(gst.LatStd));
            Assert.That(data.LonStd,             Is.EqualTo(gst.LonStd));
            Assert.That(data.AltStd,             Is.EqualTo(gst.AltStd));
            Assert.That(data.EstimatedAccuracy,  Is.EqualTo(Math.Sqrt(gst.LatStd * gst.LatStd + gst.LonStd * gst.LonStd)));
        });

        Assert.That(gst.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gst));
    }

    [Test, TestOf(nameof(GSV)), TestOf(nameof(GSVAccumulator))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetGSVMessages))]
    public void TestGSV(string[] nmeaMessages)
    {
        GPSData data = new();
        GSVAccumulator accumulator = new();
        List<Satellite> expectedSatellites = [];
        foreach (string nmea in nmeaMessages)
        {
            SetUp();
            GSV gsv = new(nmea);
            Assert.Multiple(() =>
            {
                string[] expected = nmea[..^3].Split(',');
                Assert.That(gsv.ID,               Is.EqualTo(NmeaType.GSV), FormatErrorMessage);
                Assert.That(gsv.TotalMessages,    Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                Assert.That(gsv.MessageNumber,    Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                Assert.That(gsv.SatellitesInView, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                for (int i = 0; i < gsv.Satellites.Count; i++)
                {
                    _ = int.TryParse(expected[++NmeaIndex], out int prn);
                    Assert.That(gsv.Satellites[i].PRN,       Is.EqualTo(prn), FormatErrorMessage);
                    _ = int.TryParse(expected[++NmeaIndex], out int elevation);
                    Assert.That(gsv.Satellites[i].Elevation, Is.EqualTo(elevation), FormatErrorMessage);
                    _ = int.TryParse(expected[++NmeaIndex], out int azimuth);
                    Assert.That(gsv.Satellites[i].Azimuth,   Is.EqualTo(azimuth), FormatErrorMessage);
                    _ = int.TryParse(expected[++NmeaIndex], out int snr);
                    Assert.That(gsv.Satellites[i].SNR,       Is.EqualTo(snr), FormatErrorMessage);
                }
            });

            expectedSatellites.AddRange(gsv.Satellites);
            accumulator.Apply(gsv, data);
            Assert.Multiple(() =>
            {
                bool isLastMessage = nmea == nmeaMessages[^1];
                Assert.That(data.SatellitesInView, Is.EqualTo(isLastMessage ? gsv.SatellitesInView : default));
                Assert.That(data.Satellites,       Is.EquivalentTo(isLastMessage ? expectedSatellites : []));
            });

            Assert.That(gsv.ValidateChecksum, Throws.Nothing);
            Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gsv));
        }
    }

    [Test, TestOf(nameof(RMC))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetRMCMessages))]
    public void TestRMC(string nmea)
    {
        RMC rmc = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(rmc.ID,                                Is.EqualTo(NmeaType.RMC), FormatErrorMessage);
            Assert.That(rmc.Time.ToString("HHmmss.ff"),        Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.Status,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.Lat,                               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.LatDir,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.Lon,                               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.LonDir,                            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.Speed.ToString(),                  Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.Course.ToString("##0.00"),         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.Date.ToString("ddMMyy"),           Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.MagneticVariation.ToString(),      Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.MagneticVariationDir,              Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(GetPositionModeCode(rmc.PositionMode), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rmc.NavStatus,                         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        rmc.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC,                Is.EqualTo(new DateTime(rmc.Date, rmc.Time)));
            Assert.That(data.Latitude,           Is.EqualTo(rmc.Latitude()));
            Assert.That(data.Longitude,          Is.EqualTo(rmc.Longitude()));
            Assert.That(data.PositionMode,       Is.EqualTo(rmc.PositionMode));
            Assert.That(data.Status,             Is.EqualTo(rmc.Status));
            Assert.That(data.NavStatus,          Is.EqualTo(rmc.NavStatus));
            Assert.That(data.Speed,              Is.EqualTo(rmc.Speed));
            Assert.That(data.Course,             Is.EqualTo(rmc.Course));
            Assert.That(data.MagneticVariation,  Is.EqualTo(rmc.MagneticVariation));
            Assert.That(data.MagneticVariationDir, Is.EqualTo(rmc.MagneticVariationDir));
        });

        Assert.That(rmc.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(rmc));
    }

    [Test, TestOf(nameof(RME))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetRMEMessages))]
    public void TestRME(string nmea)
    {
        Assert.Ignore("It looks like this is a proprietary message and I have not found and example to test with.");
        RME rme = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(rme.ID,                                      Is.EqualTo(NmeaType.RME), FormatErrorMessage);
            Assert.That(rme.HorizontalPositionError.ToString(),      Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rme.HorizontalPositionErrorUnits,            Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rme.VerticalPositionError.ToString(),        Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rme.VerticalPositionErrorUnits,              Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rme.SphericalPositionError.ToString(),       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(rme.SphericalPositionErrorUnits,             Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        rme.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.EstimatedAccuracy, Is.EqualTo(rme.HorizontalPositionError));
        });

        Assert.That(rme.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(rme));
    }

    [Test, TestOf(nameof(VTG))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetVTGMessages))]
    public void TestVTG(string nmea)
    {
        VTG vtg = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(vtg.ID,                                Is.EqualTo(NmeaType.VTG), FormatErrorMessage);
            Assert.That(vtg.CourseTrue.ToString("##0.00"),             Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.CourseTrueUnits,                   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.CourseMagnetic.ToString(),         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.CourseMagneticUnits,               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.SpeedKnots.ToString(),             Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.SpeedKnotsUnits,                   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.SpeedKMH.ToString(),               Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(vtg.SpeedKMHUnits,                     Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(GetPositionModeCode(vtg.PositionMode), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        vtg.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.Course,       Is.EqualTo(vtg.CourseTrue));
            Assert.That(data.Speed,        Is.EqualTo(vtg.SpeedKnots));
            Assert.That(data.PositionMode, Is.EqualTo(vtg.PositionMode));
        });

        Assert.That(vtg.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(vtg));
    }

    [Test, TestOf(nameof(ENAV))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetENAVMessages))]
    public void TestENAV(string nmea)
    {
        ENAV enav = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(enav.ID,                               Is.EqualTo(NmeaType.ENAV), FormatErrorMessage);
            Assert.That(enav.Time.ToString("HHmmss.ff"),       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.CompLat.ToString("00.00000000"),  Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.CompLon.ToString("000.00000000"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.RawLat.ToString("00.00000000"),   Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.RawLon.ToString("000.00000000"),  Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.RawAlt.ToString("0000.000"),      Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.CompAlt.ToString("0000.000"),     Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.Roll.ToString("0.00000"),         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.Pitch.ToString("0.00000"),        Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.Yaw.ToString("0.00000"),          Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.PositionQuality.ToString(),       Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            TestContext.Out.WriteLine($"Ignored enav field {++NmeaIndex}.");
            Assert.That(enav.TiltAngle.ToString(),             Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            TestContext.Out.WriteLine($"Ignored enav field {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav field {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav field {++NmeaIndex}.");
            Assert.That(enav.RawFix.ToString("d"),             Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            TestContext.Out.WriteLine($"Ignored enav field {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav field {++NmeaIndex}.");
            Assert.That(enav.AntennaHeight.ToString(),         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.LidarStrength.ToString(),         Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.EnavMode.ToString("d"),           Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.SHStatus.ToString("d"),           Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        enav.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC.TimeOfDay,      Is.EqualTo(enav.Time.ToTimeSpan()));
            Assert.That(data.Latitude,           Is.EqualTo(enav.Latitude()));
            Assert.That(data.Longitude,          Is.EqualTo(enav.Longitude()));
            Assert.That(data.Altitude,           Is.EqualTo(enav.Altitude()));
            Assert.That(data.FixQuality,         Is.EqualTo(enav.RawFix));
            Assert.That(data.EstimatedAccuracy,  Is.EqualTo(enav.PositionQuality));
            Assert.That(data.EnavData,           Is.SameAs(enav));
        });

        Assert.That(enav.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(enav));
    }
}
