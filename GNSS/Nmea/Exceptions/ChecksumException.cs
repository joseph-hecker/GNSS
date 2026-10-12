namespace GNSS.Nmea.Exceptions;

public class ChecksumException : NmeaException
{
    public ChecksumException(string sentence) : base(sentence) { }
    public ChecksumException(string message, string sentence) : base(message, sentence) { }
    public ChecksumException(string message, Exception ex, string sentence) : base(message, ex, sentence) { }
}
