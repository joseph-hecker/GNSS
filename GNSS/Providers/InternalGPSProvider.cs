namespace GNSS.Providers;

/// <summary>
/// Platform-specific GPS access (permissions, Geolocation, etc.) implemented in the host app.
/// </summary>
public interface IInternalGPSHelper
{
    void Start(Action<GPSLocation> onNewLocation);
    void Stop();
    Task<bool> EnsurePermissionsAsync(CancellationToken ct = default);
}

public class InternalGPSProvider(IInternalGPSHelper helper) : ILocationProvider
{
    public event ILocationProvider.OnNmeaRecievedEventHandler? OnNmeaRecieved = null;
    public event ILocationProvider.OnNewLocationEventHandler? OnNewLocation;

    public void Start(Dictionary<string, string>? ntripOptions = null)
    {
        helper.Start(location => OnNewLocation?.Invoke(location));
    }

    public void Stop() => helper.Stop();
}
