namespace GNSS.Nmea.Exceptions;

public class NmeaException : Exception
{
    public string Nmea { get; init; }

    public NmeaException(string nmea) : base()
    {
        Nmea = nmea.Replace("\r\n", "");
    }

    public NmeaException(string message, string nmea) : base(message)
    {
        Nmea = nmea.Replace("\r\n", "");
    }

    public NmeaException(string message, Exception ex, string nmea) : base(message, ex)
    {
        Nmea = nmea.Replace("\r\n", "");
    }
}
