using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public sealed class GSVAccumulator
{
    private GSV?[]? Parts { get; set; }

    private bool IsComplete() => Parts?.All(part => part is not null) ?? false;

    public void Apply(GSV gsv, GPSData data)
    {
        if (gsv.MessageNumber == 1 || Parts is null || Parts.Length != gsv.TotalMessages)
        {
            Parts = new GSV[gsv.TotalMessages];
        }

        Parts[gsv.MessageNumber - 1] = gsv;
        if (IsComplete())
        {
            data.SatellitesInView = Parts[0]!.SatellitesInView;
            data.Satellites = [.. Parts.SelectMany(part => part!.Satellites)];
            Parts = null;
        }
    }
}

public class GSV : NmeaSentence
{
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
        throw new InvalidOperationException(
            $"GSV sentences must be merged with {nameof(GSVAccumulator)}.{nameof(GSVAccumulator.Apply)}.");
    }
}
