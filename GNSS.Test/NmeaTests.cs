using GNSS.Nmea;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace GNSS.Test;

public class NmeaTests
{
    [Test, TestOf(typeof(GGA))]
    [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*4F")]
    [TestCase("$GPGGA,220314.00,4032.11025544,N,10502.52948864,W,0,15,0.6,1514.592,M,-20.906,M,2.0,0029*4A")]
    public void TestGGA(string nmea)
    {
        GGA gga = new(nmea);
        Assert.Multiple(() =>
        {
            string[] expected = nmea.Split(',');
            Assert.That(gga.ID, Is.EqualTo(NmeaType.GGA));
            Assert.That(gga.Time.ToString("HHmmss.ff"), Is.EqualTo(expected[1]));
            Assert.That(gga.Lat, Is.EqualTo(expected[2]));
            Assert.That(gga.LatDir, Is.EqualTo(expected[3]));
            Assert.That(gga.Lon, Is.EqualTo(expected[4]));
            Assert.That(gga.LonDir, Is.EqualTo(expected[5]));
            Assert.That(gga.Fix, Is.EqualTo(Enum.Parse<Fix>(expected[6])));
            Assert.That(gga.SatsUsed, Is.EqualTo(int.Parse(expected[7])));
            Assert.That(gga.Hdop, Is.EqualTo(double.Parse(expected[8])));
            Assert.That(gga.Alt, Is.EqualTo(double.Parse(expected[9])));
            Assert.That(gga.AltUnits, Is.EqualTo(expected[10]));
            Assert.That(gga.Undulation, Is.EqualTo(double.Parse(expected[11])));
            Assert.That(gga.UndulationUnits, Is.EqualTo(expected[12]));
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
}
