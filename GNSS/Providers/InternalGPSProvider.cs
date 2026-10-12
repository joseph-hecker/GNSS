namespace GNSS.Providers;

public interface IInternalGPSHelper
{
    void Start(Action<GPSLocation> onNewLocation);
    void Stop();
    Task<bool> EnsurePermissionsAsync(CancellationToken ct = default);
}

public class InternalGPSProvider(IInternalGPSHelper helper) : ILocationProvider
{
    public event EventHandler<object>? OnNewGPSData;

    public void Start(Dictionary<string, string>? ntripOptions = null)
    {
        helper.Start(location => OnNewGPSData?.Invoke(this, location));
    }

    public void Stop() => helper.Stop();
}
