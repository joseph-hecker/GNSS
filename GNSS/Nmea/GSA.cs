using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class GSA : NmeaSentence
{
    public GSA(string message) : base(message)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing GSA message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        throw new NotImplementedException();
    }
}
