namespace GNSS.Nmea.Exceptions;

public class NmeaException : Exception
{
    public string Nmea { get; init; }

    public NmeaException(string sentence) : base()
    {
        Nmea = sentence.Replace("\r\n", "");
    }

    public NmeaException(string message, string sentence) : base(message)
    {
        Nmea = sentence.Replace("\r\n", "");
    }

    public NmeaException(string message, Exception ex, string sentence) : base(message, ex)
    {
        Nmea = sentence.Replace("\r\n", "");
    }
}
