using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class GSA : NmeaSentence
{
    public string SelectionMode { get; }
    public FixMode FixMode { get; }
    public List<int> PRNs { get; }
    public double PDOP { get; }
    public double HDOP { get; }
    public double VDOP { get; }
    public int GNSSID { get; }

    public GSA(string message) : base(message)
    {
        try
        {
            SelectionMode = _parts[1];
            FixMode = (FixMode)int.Parse(_parts[2]);
            PRNs = [];
            for (int i = 3; i < _parts.Length - 4; i++)
            {
                if (int.TryParse(_parts[i], out int prn))
                {
                    PRNs.Add(prn);
                }
            }
            VDOP = double.Parse(_parts[15]);
            HDOP = double.Parse(_parts[16]);
            PDOP = double.Parse(_parts[17]);
            GNSSID = int.Parse(_parts[18]);
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing GSA message.", ex, message);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        data.FixMode = FixMode;
        data.PDOP = PDOP;
        data.HDOP = HDOP;
        data.VDOP = VDOP;
    }
}
