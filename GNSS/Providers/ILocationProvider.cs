namespace GNSS.Providers;

public interface ILocationProvider
{
    public delegate void OnNmeaRecievedEventHandler(string nmea);
    public event OnNmeaRecievedEventHandler? OnNmeaRecieved;

    public delegate void OnNewLocationEventHandler(GPSLocation location);
    public event OnNewLocationEventHandler? OnNewLocation;

    public void Start(Dictionary<string, string>? ntripOptions = null);
    public void Stop();
}
