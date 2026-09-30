using GNSS.Nmea;
using GNSS.Nmea.Exceptions;
using System.Data;

namespace GNSS.Test;

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

    private static string[] GetInvalidMedssageForEachNmeaType()
    {
        return [.. from id in Enum.GetValues<NmeaType>().Where(nmeaType => nmeaType != NmeaType.Unknown) select $"${id},,,,,,*00"];
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
            Assert.That(gga.FixQuality, Is.EqualTo(Enum.Parse<FixQuality>(expected[++NmeaIndex])), FormatErrorMessage);
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
            Assert.That(data.FixQuality, Is.EqualTo(gga.FixQuality));
            Assert.That(data.HDOP, Is.EqualTo(gga.Hdop));
            Assert.That(data.SatellitesInUse, Is.EqualTo(gga.SatsUsed));
            Assert.That(data.Age, Is.EqualTo(gga.Age));
            Assert.That(data.StationID, Is.EqualTo(gga.StationID));
        });

        Assert.That(gga.ValidateChecksum, Throws.Nothing);
        Assert.That(NmeaSentence.Parse(nmea), Is.EqualTo(gga));
    }

    [Test, TestOf(nameof(GSV))]
    [TestCase("$GPGSV,8,1,25,21,44,141,47,15,14,049,44,6,31,255,46,3,25,280,44*75\r\n" +
              "$GPGSV,8,2,25,18,61,057,48,22,68,320,52,27,34,268,47,24,32,076,45*76\r\n" +
              "$GPGSV,8,3,25,14,51,214,49,19,23,308,46*7E\r\n" +
              "$GPGSV,8,4,25,51,44,183,49,46,41,169,43,48,36,220,45*47\r\n" +
              "$GLGSV,8,5,25,82,49,219,52,76,22,051,41,83,37,316,51,67,57,010,51*6C\r\n" +
              "$GLGSV,8,6,25,77,24,108,44,81,10,181,46,78,1,152,34,66,18,060,45*50\r\n" +
              "$GLGSV,8,7,25,68,37,284,50*5C\r\n" +
              "$GBGSV,8,8,25,111,35,221,47,112,4,179,39,114,48,290,48*55\r\n")]
    public void TestGSV(string nmea)
    {
        GPSData data = new();
        GSVAccumulator accumulator = new();
        List<Satellite> expectedSatellites = [];
        string[] messages = nmea.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        foreach (string message in messages)
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
            accumulator.Apply(gsv, data);
            Assert.Multiple(() =>
            {
                bool isLastMessage = message == messages[^1];
                Assert.That(data.SatellitesInView, Is.EqualTo(isLastMessage ? gsv.SatellitesInView : default));
                Assert.That(data.Satellites, Is.EquivalentTo(isLastMessage ? expectedSatellites : []));
            });

            Assert.That(gsv.ValidateChecksum, Throws.Nothing);
            Assert.That(NmeaSentence.Parse(message), Is.EqualTo(gsv));
        }
    }

    [Test, TestOf(nameof(NmeaSentence.Parse))]
    [TestCase("$GPXXX,123456.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7A")]
    public void TestParseUnknownSentence(string nmea)
    {
        Assert.That(NmeaSentence.Parse(nmea), Is.Null);
    }

    [Test, TestOf(nameof(NmeaSentence.Parse))]
    [TestCase("This is not a nmea sentence.")]
    [TestCaseSource(nameof(GetInvalidMedssageForEachNmeaType))]
    public void TestParseInvalidSentence(string nmea)
    {
        Assert.Throws<NmeaException>(() => NmeaSentence.Parse(nmea));
    }

    [Test, TestOf(nameof(NmeaSentence.ValidateChecksum))]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7F", true)]
    [TestCase("$GPGGA,220314.00,4032.11025544,N,10502.52948864,W,0,15,0.6,1514.592,M,-20.906,M,2.0,0029*4A", true)]
    [TestCase("$GPGSV,8,1,25,21,44,141,47,15,14,049,44,6,31,255,46,3,25,280,44*75", true)]
    [TestCase("$GPGSV,8,2,25,18,61,057,48,22,68,320,52,27,34,268,47,24,32,076,45*76", true)]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*00", false)]
    [TestCase("$GPGGA,220314.00,4032.11025544,N,10502.52948864,W,0,15,0.6,1514.592,M,-20.906,M,2.0,0029*4B", false)]
    [TestCase("$GPGSV,8,1,25,21,44,141,47,15,14,049,44,6,31,255,46,3,25,280,44*32", false)]
    [TestCase("$GPGSV,8,2,25,18,61,057,48,22,68,320,52,27,34,268,47,24,32,076,45*FF", false)]
    public void TestValidateChecksum(string nmea, bool isValid)
    {
        Assert.That(() => NmeaSentence.Parse(nmea), isValid ? Throws.Nothing : Throws.TypeOf<ChecksumException>());
    }

    [Test, TestOf(nameof(NmeaSentence.Equals)), TestOf(nameof(NmeaSentence.GetHashCode)), TestOf(nameof(NmeaSentence.ToString))]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7F",
              "$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7F")]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7F",
              "$GPGGA,220314.00,4032.11025544,N,10502.52948864,W,0,15,0.6,1514.592,M,-20.906,M,2.0,0029*4A")]
    public void TestNmeaEquality(string nmea1, string nmea2)
    {
        bool expected = nmea1 == nmea2;
        NmeaSentence sentence1 = NmeaSentence.Parse(nmea1)!;
        NmeaSentence sentence2 = NmeaSentence.Parse(nmea2)!;
        Assert.Multiple(() =>
        {
            bool checkEquality = sentence1.Equals(sentence2);
            bool checkObjEquality = sentence1.Equals((object?)sentence2);
            bool checkHashCode = sentence1.GetHashCode() == sentence2.GetHashCode();
            bool checkToString = sentence1.ToString() == sentence2.ToString();
            Assert.That(checkEquality, Is.EqualTo(expected));
            Assert.That(checkObjEquality, Is.EqualTo(expected));
            Assert.That(checkHashCode, Is.EqualTo(expected));
            Assert.That(checkToString, Is.EqualTo(expected));
        });
    }
}
