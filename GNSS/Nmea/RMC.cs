using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class RMC : NmeaSentence
{
    public RMC(string message) : base(message)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing RMC message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        throw new NotImplementedException();
    }
}
