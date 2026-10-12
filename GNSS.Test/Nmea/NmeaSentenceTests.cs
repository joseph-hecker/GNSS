using GNSS.Nmea;
using GNSS.Nmea.Exceptions;

namespace GNSS.Test.Nmea;

[TestFixture, TestOf(nameof(NmeaSentence))]
public class NmeaSentenceTests
{
    private static string[] GetInvalidMedssageForEachNmeaType()
    {
        IEnumerable<NmeaType> nmeaTypes = Enum.GetValues<NmeaType>().Where(nmeaType => nmeaType != NmeaType.Unknown);
        return [.. nmeaTypes.Select(id =>
        {
            string message = $"${id},,,,,,,,";
            return message + $"*{Util.GenerateChecksum(message)}";
        })];
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
        NmeaException? ex = Assert.Throws<NmeaException>(() => NmeaSentence.Parse(nmea));
        Assert.That(ex, Is.Not.TypeOf<ChecksumException>());
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
