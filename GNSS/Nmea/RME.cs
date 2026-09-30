using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

/// <summary>
/// NOTE <br/><br/>
/// I could only get information for RME messages from the CartoPac repo and couldn't find anything on google so this implementation may be incomplete.
/// </summary>
public class RME : NmeaSentence
{
    public double HorizontalPositionError { get; }
    public string HorizontalPositionErrorUnits { get; }
    public double VerticalPositionError { get; }
    public string VerticalPositionErrorUnits { get; }
    public double SphericalPositionError { get; }
    public string SphericalPositionErrorUnits { get; }

    public RME(string message) : base(message)
    {
        try
        {
            HorizontalPositionError = double.Parse(_parts[1]);
            HorizontalPositionErrorUnits = _parts[2];
            VerticalPositionError = double.Parse(_parts[3]);
            VerticalPositionErrorUnits = _parts[4];
            SphericalPositionError = double.Parse(_parts[5]);
            SphericalPositionErrorUnits = _parts[6];
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
