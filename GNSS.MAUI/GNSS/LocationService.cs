using GNSS;
using GNSS.MAUI.GNSS;

namespace CartoPac_Mobile.GNSS;

public class LocationService
{
    public static readonly Dictionary<ProviderType, string> AvailableProviders = new()
    {
        { ProviderType.InternalGPS, "Internal GPS" },
        { ProviderType.Bluetooth, "Bluetooth" },
        { ProviderType.TCP, "TCP" },
        { ProviderType.TextFile, "Text File" }
    };

    private IMapCenterSource? MapCenterSource { get; set; }
    private InternalGpsHelper InternalGPSHelper { get; } = new();

    private readonly static GPS _mapGPS = new();
    private readonly static GPS _deviceGPS = new();
    private GPS ActiveGPS { get; set; } = _mapGPS;

    public bool IsDeviceGnssActive => ReferenceEquals(ActiveGPS, _deviceGPS);

    public event GPS.OnLocationUpdatedEventHandler? OnLocationUpdated
    {
        add
        {
            _mapGPS.OnLocationUpdated += value;
            _deviceGPS.OnLocationUpdated += value;
        }

        remove
        {
            _mapGPS.OnLocationUpdated -= value;
            _deviceGPS.OnLocationUpdated -= value;
        }
    }

    public void RegisterMapCenterSource(IMapCenterSource source)
    {
        MapCenterSource = source;
        _mapGPS.Connect(new MapHelper(MapCenterSource.TryGetMapCenter));
        _mapGPS.StartUpdatingLocation();
    }

    public void UnregisterMapCenterSource(IMapCenterSource source)
    {
        if (ReferenceEquals(MapCenterSource, source))
        {
            _mapGPS.Disconnect();
            MapCenterSource = null;
        }
    }

    public void ConnectDeviceGPS()
    {
        ProviderType provider;
        switch (provider = GetLocationProviderSettings(out Dictionary<string, string>? options))
        {

            case ProviderType.InternalGPS:
                _deviceGPS.Connect(InternalGPSHelper);
                break;

            case ProviderType.Bluetooth:
            case ProviderType.TCP:
            case ProviderType.TextFile:
                if (options is null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                _deviceGPS.Connect(provider, options);
                break;

            case ProviderType.Map:
            default:
                throw new InvalidOperationException($"'{nameof(_deviceGPS)}' cannot use the map location provider.");
        }
    }

    public void DisconnectDeviceGPS()
    {
        _deviceGPS.Disconnect();
    }

    public void EnableGNSS()
    {
        ActiveGPS.StopUpdatingLocation();
        ActiveGPS = _deviceGPS;
        ActiveGPS.StartUpdatingLocation();
    }

    public void DisableGNSS()
    {
        ActiveGPS.StopUpdatingLocation();
        ActiveGPS = _mapGPS;
        ActiveGPS.StartUpdatingLocation();
    }

    private ProviderType GetLocationProviderSettings(out Dictionary<string, string>? providerOptions)
    {
        ProviderType provider = Enum.Parse<ProviderType>(Preferences.Get(Options.Provider, string.Empty));
        providerOptions = provider switch
        {
            ProviderType.TCP => new Dictionary<string, string>
            {
                { Options.Host, Preferences.Get(Options.Host, string.Empty) },
                { Options.Port, Preferences.Get(Options.Port, string.Empty) },
            },
            ProviderType.Bluetooth => new Dictionary<string, string>
            {
                { Options.COMPort, Preferences.Get(Options.COMPort, string.Empty) },
                { Options.BaudRate, Preferences.Get(Options.BaudRate, string.Empty) },
            },
            ProviderType.TextFile => new Dictionary<string, string>
            {
                { Options.NmeaFile, Preferences.Get(Options.NmeaFile, string.Empty) },
            },
            _ => null
        };

        return provider;
    }
}
