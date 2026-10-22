namespace GNSS.Providers;

public interface IInternalGPSHelper
{
    void Start(Action<GPSLocation> onNewLocation);
    void Stop();
    Task<bool> EnsurePermissionsAsync(CancellationToken ct = default);
}

public class InternalGPSProvider(IInternalGPSHelper helper) : ILocationProvider
{
    public ProviderType Type => ProviderType.InternalGPS;

    public event EventHandler<object>? OnNewGPSData;

    public async void Start(Dictionary<string, string>? ntripOptions = null)
    {
        if (await helper.EnsurePermissionsAsync())
            helper.Start(location => OnNewGPSData?.Invoke(this, location));
    }

    public void Stop() => helper.Stop();
}
