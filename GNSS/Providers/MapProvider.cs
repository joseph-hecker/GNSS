namespace GNSS.Providers;

/// <summary>
/// Map view access (camera center, visible region, etc.) implemented in the host app.
/// </summary>
public interface IMapHelper
{
    void Start(Action<GPSLocation> onNewLocation);
    void Stop();
}

public class MapProvider(IMapHelper helper) : ILocationProvider
{
    public event EventHandler<object>? OnNewGPSData;

    public void Start(Dictionary<string, string>? ntripOptions = null)
    {
        helper.Start(location => OnNewGPSData?.Invoke(this, location));
    }

    public void Stop() => helper.Stop();
}
