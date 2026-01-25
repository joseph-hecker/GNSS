using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class GSV : NmeaSentence
{
    private static GSV[] CompleteSentence { get; set; } = [];

    public int TotalMessages { get; }
    public int MessageNumber { get; }
    public int SatellitesInView { get; }
    public List<Satellite> Satellites { get; }

    public GSV(string message) : base(message)
    {
        try
        {
            TotalMessages = int.Parse(_parts[1]);
            MessageNumber = int.Parse(_parts[2]);
            SatellitesInView = int.Parse(_parts[3]);
            Satellites = [];
            for (int i = 4; i < _parts.Length - 1; i += 4)
            {
                _ = int.TryParse(_parts[i], out int prn);
                _ = int.TryParse(_parts[i + 1], out int elevation);
                _ = int.TryParse(_parts[i + 2], out int azimuth);
                _ = int.TryParse(_parts[i + 3], out int snr);
                if (prn != default || elevation != default || azimuth != default || snr != default)
                {
                    Satellites.Add(new(prn, elevation, azimuth, snr));
                }
            }
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing GSV message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        if (MessageNumber == 1)
        {
            CompleteSentence = new GSV[TotalMessages];
            CompleteSentence[0] = this;
        }
        else
        {
            CompleteSentence[MessageNumber - 1] = this;
        }

        if (MessageNumber == TotalMessages)
        {
            data.SatellitesInView = CompleteSentence[0].SatellitesInView;
            data.Satellites = CompleteSentence.SelectMany(gsv => gsv.Satellites);
        }
    }
}
