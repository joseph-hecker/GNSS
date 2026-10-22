namespace GNSS.Providers;

public interface ILocationProvider
{
    public ProviderType Type { get; }

    public event EventHandler<object>? OnNewGPSData;

    public void Start(Dictionary<string, string>? ntripOptions = null);
    public void Stop();
}
