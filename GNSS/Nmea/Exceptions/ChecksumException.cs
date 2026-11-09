namespace GNSS.Nmea.Exceptions;

public class ChecksumException : NmeaException
{
    public ChecksumException(string nmea) : base(nmea) { }
    public ChecksumException(string message, string nmea) : base(message, nmea) { }
    public ChecksumException(string message, Exception ex, string nmea) : base(message, ex, nmea) { }
}
