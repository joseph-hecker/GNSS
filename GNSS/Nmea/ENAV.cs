using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class ENAV : NmeaSentence
{
    public TimeOnly Time { get; }
    public double CompLatitude { get; }
    public double CompLongitude { get; }
    public double RawLatitude { get; }
    public double RawLongitude { get; }
    public double RawAltitude { get; }
    public double CompAltitude { get; }

    public bool ValidCompData
    {
        get
        {
            double min = -0.00005, max = 0.00005;
            return !(min < CompLatitude && CompLatitude < max) &&
                   !(min < CompLongitude && CompLongitude < max);
        }
    }

    public ENAV(string message) : base(message)
    {
        try
        {

        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing ENAV message. ({message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        throw new NotImplementedException();
    }
}
