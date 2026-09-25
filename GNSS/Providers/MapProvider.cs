namespace GNSS.Providers;

/// <summary>
/// Map view access (camera center, visible region, etc.) implemented in the host app.
/// </summary>
public interface IMapHelper
{
    void Start(Action<GPSLocation> onLocation);
    void Stop();
}

public class MapProvider(IMapHelper helper) : ILocationProvider
{
    public event ILocationProvider.OnNmeaRecievedEventHandler? OnNmeaRecieved = null;
    public event ILocationProvider.OnNewLocationEventHandler? OnNewLocation;

    public void Start(Dictionary<string, string>? ntripOptions = null)
    {
        helper.Start(location => OnNewLocation?.Invoke(location));
    }

    public void Stop() => helper.Stop();
}
