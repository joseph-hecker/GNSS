namespace GNSS.Drivers;

public interface IDriver
{
    public delegate void OnNmeaRecievedEventHandler(string nmea);
    public event OnNmeaRecievedEventHandler? OnNmeaRecieved;

    public double PoleHeight { get; set; }

    public void Start(Dictionary<string, string>? ntripOptions);
    public void Stop();
}
