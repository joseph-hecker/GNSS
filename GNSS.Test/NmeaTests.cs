using GNSS.Nmea;
using System.Collections;

namespace GNSS.Test;

public class NmeaTests
{
    private static string FormatErrorMessage(string nmeaMessage, int e)
    {
        return $"Error parsing nmea message at element {e}. ({nmeaMessage})";
    }

    [Test, TestOf(nameof(GGA))]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7F")]
    [TestCase("$GPGGA,220314.00,4032.11025544,N,10502.52948864,W,0,15,0.6,1514.592,M,-20.906,M,2.0,0029*4A")]
    public void TestGGA(string nmea)
    {
        GGA gga = new(nmea);
        Assert.Multiple(() =>
        {
            int i = 0;
            string[] expected = nmea[..^3].Split(',');
            Assert.That(gga.ID, Is.EqualTo(NmeaType.GGA), FormatErrorMessage(nmea, i));
            Assert.That(gga.Time.ToString("HHmmss.ff"), Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
            Assert.That(gga.Lat, Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
            Assert.That(gga.LatDir, Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
            Assert.That(gga.Lon, Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
            Assert.That(gga.LonDir, Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
            Assert.That(gga.Fix, Is.EqualTo(Enum.Parse<Fix>(expected[++i])), FormatErrorMessage(nmea, i));
            Assert.That(gga.SatsUsed, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(nmea, i));
            Assert.That(gga.Hdop, Is.EqualTo(double.Parse(expected[++i])), FormatErrorMessage(nmea, i));
            Assert.That(gga.Alt, Is.EqualTo(double.Parse(expected[++i])), FormatErrorMessage(nmea, i));
            Assert.That(gga.AltUnits, Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
            Assert.That(gga.Undulation, Is.EqualTo(double.Parse(expected[++i])), FormatErrorMessage(nmea, i));
            Assert.That(gga.UndulationUnits, Is.EqualTo(expected[++i]), FormatErrorMessage(nmea, i));
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
        List<Satellite> expectedSatellites = [];
        string[] messages = nmea.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        foreach (string message in messages)
        {
            GSV gsv = new(message);
            Assert.Multiple(() =>
            {
                int i = 0;
                string[] expected = message[..^3].Split(',');
                Assert.That(gsv.ID, Is.EqualTo(NmeaType.GSV), FormatErrorMessage(message, i));
                Assert.That(gsv.TotalMessages, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                Assert.That(gsv.MessageNumber, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                Assert.That(gsv.SatellitesInView, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                foreach (Satellite satellite in gsv.Satellites)
                {
                    Assert.That(satellite.PRN, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                    Assert.That(satellite.Elevation, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                    Assert.That(satellite.Azimuth, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                    Assert.That(satellite.SNR, Is.EqualTo(int.Parse(expected[++i])), FormatErrorMessage(message, i));
                }
            });

            expectedSatellites.AddRange(gsv.Satellites);
            gsv.UpdateGPSData(data);
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
}
