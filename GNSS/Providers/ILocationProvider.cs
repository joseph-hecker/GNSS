namespace GNSS.Providers;

public interface ILocationProvider
{
    public event EventHandler<object>? OnNewGPSData;

    public void Start(Dictionary<string, string>? ntripOptions = null);
    public void Stop();
}
