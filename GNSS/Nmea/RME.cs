using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class RME : NmeaSentence
{
    public RME(string message) : base(message)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing RME message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        throw new NotImplementedException();
    }
}
