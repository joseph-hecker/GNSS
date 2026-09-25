using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class GST : NmeaSentence
{
    public GST(string message) : base(message)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing GST message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        throw new NotImplementedException();
    }
}
