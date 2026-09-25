namespace GNSS.Providers;

public interface ILocationProvider
{
    public delegate void OnNmeaRecievedEventHandler(string nmea);
    public event OnNmeaRecievedEventHandler? OnNmeaRecieved;

    public void Start(Dictionary<string, string>? ntripOptions = null);
    public void Stop();
}
