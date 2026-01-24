namespace GNSS.Drivers;

public interface IDriver
{
    public delegate void OnNmeaRecievedEventHandler(string nmea);
    public event OnNmeaRecievedEventHandler? OnNmeaRecieved;

    public void Start(Dictionary<string, string>? ntripOptions = null);
    public void Stop();
}
