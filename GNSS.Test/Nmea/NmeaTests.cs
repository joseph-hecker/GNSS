using GNSS.Nmea;

namespace GNSS.Test.Nmea;

public class NmeaTests
{
    private static int NmeaIndex { get; set; }

    private static string FormatErrorMessage()
    {
        string? nmea = TestContext.CurrentContext.Test.Arguments.SingleOrDefault(arg => arg is string str && str.StartsWith('$')) as string;
        string message = $"Error parsing nmea message at element {NmeaIndex}.";
        if (nmea is not null)
        {
            message += $" ({nmea})";
        }

        return message;
    }

    private static string[][] GetGSVMessages()
    {
        return
        [
            [
                "$GPGSV,8,1,25,21,44,141,47,15,14,049,44,6,31,255,46,3,25,280,44*75",
                "$GPGSV,8,2,25,18,61,057,48,22,68,320,52,27,34,268,47,24,32,076,45*76",
                "$GPGSV,8,3,25,14,51,214,49,19,23,308,46*7E",
                "$GPGSV,8,4,25,51,44,183,49,46,41,169,43,48,36,220,45*47",
                "$GLGSV,8,5,25,82,49,219,52,76,22,051,41,83,37,316,51,67,57,010,51*6C",
                "$GLGSV,8,6,25,77,24,108,44,81,10,181,46,78,1,152,34,66,18,060,45*50",
                "$GLGSV,8,7,25,68,37,284,50*5C",
                "$GBGSV,8,8,25,111,35,221,47,112,4,179,39,114,48,290,48*55",
            ],
        ];
    }

    [SetUp]
    public void SetUp()
    {
        NmeaIndex = 0;
    }

    [Test, TestOf(nameof(GGA))]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7F")]
    [TestCase("$GPGGA,220314.00,4032.11025544,N,10502.52948864,W,0,15,0.6,1514.592,M,-20.906,M,2.0,0029*4A")]
    public void TestGGA(string nmea)
    {
        GGA gga = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(gga.ID, Is.EqualTo(NmeaType.GGA), FormatErrorMessage);
            Assert.That(gga.Time.ToString("HHmmss.ff"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Lat, Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.LatDir, Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Lon, Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.LonDir, Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Fix, Is.EqualTo(Enum.Parse<Fix>(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(gga.SatsUsed, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(gga.Hdop, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(gga.Alt, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(gga.AltUnits, Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(gga.Undulation, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(gga.UndulationUnits, Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
        });

        GPSData data = new();
        gga.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC.TimeOfDay, Is.EqualTo(gga.Time.ToTimeSpan()));
            Assert.That(data.Latitude, Is.EqualTo(gga.Latitude()));
            Assert.That(data.Longitude, Is.EqualTo(gga.Longitude()));
            Assert.That(data.Altitude, Is.EqualTo(gga.Alt));
            Assert.That(data.Fix, Is.EqualTo(gga.Fix));
            Assert.That(data.HDOP, Is.EqualTo(gga.Hdop));
            Assert.That(data.SatellitesInUse, Is.EqualTo(gga.SatsUsed));
            Assert.That(data.Age, Is.EqualTo(gga.Age));
            Assert.That(data.StationID, Is.EqualTo(gga.StationID));
        });

        Assert.That(gga.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gga));
    }

    [Test, TestOf(nameof(GLL))]
    [TestCase("$GPGLL,3953.88008971,N,10506.75318910,W,034138.00,A,D*7A")]
    public void TestGLL(string nmea)
    {
        Assert.Ignore();
        GLL gll = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
        });

        GPSData data = new();
        gll.UpdateGPSData(data);
        Assert.Multiple(() =>
        {

        });

        Assert.That(gll.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gll));
    }

    [Test, TestOf(nameof(GSA))]
    [TestCase("$GNGSA,A,3,21,5,29,25,12,10,26,2,,,,,1.2,0.7,1.0*27")]
    [TestCase("$GNGSA,A,3,65,67,80,81,82,88,66,,,,,,1.2,0.7,1.0*20")]
    public void TestGSA(string nmea)
    {
        Assert.Ignore();
        GSA gsa = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
        });

        GPSData data = new();
        gsa.UpdateGPSData(data);
        Assert.Multiple(() =>
        {

        });

        Assert.That(gsa.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gsa));
    }

    [Test, TestOf(nameof(GST))]
    [TestCase("$GPGST,172814.0,0.006,0.023,0.020,273.6,0.023,0.020,0.031*6A")]
    public void TestGST(string nmea)
    {
        Assert.Ignore();
        GST gst = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
        });

        GPSData data = new();
        gst.UpdateGPSData(data);
        Assert.Multiple(() =>
        {

        });

        Assert.That(gst.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gst));
    }

    [Test, TestOf(nameof(GSV))]
    [TestCaseSource(nameof(GetGSVMessages))]
    public void TestGSV(string[] nmeaMessages)
    {
        GPSData data = new();
        List<Satellite> expectedSatellites = [];
        foreach (string message in nmeaMessages)
        {
            SetUp();
            GSV gsv = new(message);
            Assert.Multiple(() =>
            {
                string[] expected = message[..^3].Split(',');
                Assert.That(gsv.ID, Is.EqualTo(NmeaType.GSV), FormatErrorMessage);
                Assert.That(gsv.TotalMessages, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                Assert.That(gsv.MessageNumber, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                Assert.That(gsv.SatellitesInView, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                foreach (Satellite satellite in gsv.Satellites)
                {
                    Assert.That(satellite.PRN, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                    Assert.That(satellite.Elevation, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                    Assert.That(satellite.Azimuth, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                    Assert.That(satellite.SNR, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
                }
            });

            expectedSatellites.AddRange(gsv.Satellites);
            gsv.UpdateGPSData(data);
            Assert.Multiple(() =>
            {
                bool isLastMessage = message == nmeaMessages[^1];
                Assert.That(data.SatellitesInView, Is.EqualTo(isLastMessage ? gsv.SatellitesInView : default));
                Assert.That(data.Satellites, Is.EquivalentTo(isLastMessage ? expectedSatellites : []));
            });

            Assert.That(gsv.ValidateChecksum, Throws.Nothing);
            Assert.That(NmeaSentence.Parse(message), Is.EqualTo(gsv));
        }
    }

    [Test, TestOf(nameof(RMC))]
    [TestCase("$GPRMC,123519,A,4807.038,N,01131.000,E,022.4,084.4,230394,003.1,W*6A")]
    public void TestRMC(string nmea)
    {
        Assert.Ignore();
        RMC rmc = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
        });

        GPSData data = new();
        rmc.UpdateGPSData(data);
        Assert.Multiple(() =>
        {

        });

        Assert.That(rmc.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(rmc));
    }

    [Test, TestOf(nameof(RMC))]
    [TestCase("")]
    [TestCase("")]
    public void TestRME(string nmea)
    {
        Assert.Ignore();
        RME rme = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
        });

        GPSData data = new();
        rme.UpdateGPSData(data);
        Assert.Multiple(() =>
        {

        });

        Assert.That(rme.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(rme));
    }

    [Test, TestOf(nameof(VTG))]
    [TestCase("")]
    [TestCase("")]
    public void TestVTG(string nmea)
    {
        Assert.Ignore();
        VTG vtg = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
        });

        GPSData data = new();
        vtg.UpdateGPSData(data);
        Assert.Multiple(() =>
        {

        });

        Assert.That(vtg.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(vtg));
    }

    [Test, TestOf(nameof(ENAV))]
    [TestCase("$ENAV,195252.10,40.53495815,-105.04241294,40.53496139,-105.04241390,1495.534,1493.514,-0.16472,-0.01528,1.21854,0.012,0.13,0.165,0.268,0x0,25,4,2.0,0x1e0014,2052,9999,0,2,0*65")]
    [TestCase("$ENAV,195259.70,40.53496318,-105.04240997,40.53496517,-105.04241468,1495.387,1493.389,-0.21022,0.08154,0.86950,0.022,0.13,0.225,0.278,0x0,25,4,3.0,0x1e0014,2052,9999,0,2,0*43")]
    public void TestENAV(string nmea)
    {
        ENAV enav = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea[..^3].Split(',');
            Assert.That(enav.ID, Is.EqualTo(NmeaType.ENAV), FormatErrorMessage);
            Assert.That(enav.Time.ToString("HHmmss.ff"), Is.EqualTo(expected[++NmeaIndex]), FormatErrorMessage);
            Assert.That(enav.CompLatitude, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.CompLongitude, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.RawLatitude, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.RawLongitude, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.RawAltitude, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.CompAltitude, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.Roll, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.Pitch, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.Yaw, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.PositionQuality, Is.EqualTo(double.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            Assert.That(enav.RawFix, Is.EqualTo(Enum.Parse<Fix>(expected[++NmeaIndex])), FormatErrorMessage);
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            TestContext.Out.WriteLine($"Ignored enav element {++NmeaIndex}.");
            Assert.That(enav.AntennaHeight, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.LidarStrength, Is.EqualTo(int.Parse(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.EnavMode, Is.EqualTo(Enum.Parse<EnavMode>(expected[++NmeaIndex])), FormatErrorMessage);
            Assert.That(enav.SHStatus, Is.EqualTo(Enum.Parse<SmartHandleStatus>(expected[++NmeaIndex])), FormatErrorMessage);
        });

        GPSData data = new();
        enav.UpdateGPSData(data);
        Assert.Multiple(() =>
        {
            Assert.That(data.UTC.TimeOfDay, Is.EqualTo(enav.Time.ToTimeSpan()));
            Assert.That(data.Latitude, Is.EqualTo(enav.Latitude()));
            Assert.That(data.Longitude, Is.EqualTo(enav.Longitude()));
            Assert.That(data.Altitude, Is.EqualTo(enav.Altitude()));
            Assert.That(data.Fix, Is.EqualTo(enav.RawFix));
            Assert.That(data.EstimatedAccuracy, Is.EqualTo(enav.PositionQuality));
            Assert.That(data.EnavData, Is.SameAs(enav));
        });

        Assert.That(enav.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(enav));
    }
}


