using GNSS.Nmea;
using GNSS.Nmea.Exceptions;

namespace GNSS.Test.Nmea;

[TestFixture, TestOf(nameof(NmeaSentence))]
public class NmeaSentenceTests
{

    [Test, TestOf(nameof(NmeaSentence.Parse))]
    [TestCase("$GPXXX,123456.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031*7A")]
    public void TestParseUnknownSentence(string nmea)
    {
        Assert.That(NmeaSentence.Parse(nmea), Is.Null);
    }

    [Test, TestOf(nameof(NmeaSentence.Parse))]
    [TestCase("This is not a nmea sentence.")]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetInvalidMedssageForEachNmeaType))]
    public void TestParseInvalidSentence(string nmea)
    {
        NmeaException? ex = Assert.Throws<NmeaException>(() => NmeaSentence.Parse(nmea));
        Assert.That(ex, Is.Not.TypeOf<ChecksumException>());
    }

    [Test, TestOf(nameof(NmeaSentence.ValidateChecksum))]
    [TestCaseSource(typeof(NmeaSource), nameof(NmeaSource.GetNmeaMessagesForChecksum))]
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
