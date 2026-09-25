using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class VTG : NmeaSentence
{
    public VTG(string message) : base(message)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing VTG message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        throw new NotImplementedException();
    }
}
