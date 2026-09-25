namespace GNSS.MAUI.GNSS;

public sealed class LocationService
{
    private IMapCenterSource? MapCenterSource { get; set; }
    private InternalGpsHelper InternalGPSHelper { get; } = new();

    public event GPS.OnLocationUpdatedEventHandler? OnLocationUpdated
    {
        add => GPS.OnLocationUpdated += value;
        remove => GPS.OnLocationUpdated -= value;
    }

    public void RegisterMapCenterSource(IMapCenterSource source) => MapCenterSource = source;

    public void UnregisterMapCenterSource(IMapCenterSource source)
    {
        if (ReferenceEquals(MapCenterSource, source))
        {
            MapCenterSource = null;
        }
    }

    public void Connect()
    {
        ProviderType provider;
        switch (provider = GetLocationProviderSettings(out Dictionary<string, string>? options))
        {
            case ProviderType.Map:
                if (MapCenterSource is null)
                {
                    throw new ArgumentNullException(nameof(MapCenterSource));
                }

                GPS.Connect(new MapHelper(MapCenterSource.TryGetMapCenter));
                break;

            case ProviderType.InternalGps:
                GPS.Connect(InternalGPSHelper);
                break;

            default:
                if (options is null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                GPS.Connect(provider, options);
                break;
        }
    }

    public void Disconnect()
    {
        GPS.Disconnect();
    }

    public void EnableGNSS()
    {
        GPS.StartUpdatingLocation();
    }

    public void DisableGNSS()
    {
        GPS.StopUpdatingLocation();
    }

    private ProviderType GetLocationProviderSettings(out Dictionary<string, string>? providerOptions)
    {
        providerOptions = null;

        throw new NotImplementedException();
    }

}
