using GNSS.Nmea;
using GNSS.Nmea.Exceptions;
using GNSS.Providers;
using System.Diagnostics;

namespace GNSS;

public static class GPS
{
    private static ILocationProvider? LocationProvider { get; set; }
    private static GPSData? CurrentGPSData { get; set; }
    private static Timer? LocationTimer { get; set; }

    public delegate void OnLocationUpdatedEventHandler(GPSData data);
    public static event OnLocationUpdatedEventHandler? OnLocationUpdated;

    public static async Task<GPSData> GetNextLocationAsync()
    {
        if (CurrentGPSData is null)
        {
            throw new InvalidOperationException($"GPS is not streaming data, call {nameof(Connect)}() and {nameof(StartUpdatingLocation)}() first.");
        }

        TaskCompletionSource<GPSData> source = new();
        void SetResult(GPSData data)
        {
            OnLocationUpdated -= SetResult;
            source.SetResult(data);
        }

        OnLocationUpdated += SetResult;
        return await source.Task;
    }

    private static void OnNmeaRecieved(string nmea)
    {
        try
        {
            NmeaSentence? sentence = NmeaSentence.Parse(nmea);
            if (sentence is not null && CurrentGPSData is not null)
            {
                sentence.UpdateGPSData(CurrentGPSData);
            }
        }
        catch (ChecksumException) { }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private static void OnNewLocation(GPSLocation location)
    {
        if (CurrentGPSData is not null)
        {
            location.UpdateGPSData(CurrentGPSData!);
        }
    }

    private static void Connect(ILocationProvider provider)
    {
        Disconnect();
        LocationProvider = provider;
    }

    public static void Connect(ProviderType providerType, Dictionary<string, string> providerOptions)
    {
        Connect(providerType switch
        {
            ProviderType.TCP => new TCPProvider(providerOptions[Options.Host], int.Parse(providerOptions[Options.Port])),
            ProviderType.Bluetooth => new BluetoothProvider(providerOptions[Options.COMPort], int.Parse(providerOptions[Options.BaudRate])),
            ProviderType.TextFile => new TextFileProvider(providerOptions[Options.NmeaFile]),
            _ => throw new InvalidOperationException($"'{providerType}' GPS provider is not supported."),
        });
    }

    public static void Connect(IInternalGPSHelper helper) => Connect(new InternalGPSProvider(helper));

    public static void Connect(IMapHelper helper) => Connect(new MapProvider(helper));

    public static void Disconnect()
    {
        StopUpdatingLocation();
        LocationProvider = null;
    }

    public static void StartUpdatingLocation(Dictionary<string, string>? ntripOptions = null)
    {
        if (LocationProvider is null)
        {
            throw new InvalidOperationException("GPS provider is not connected. Call Connect() first.");
        }

        CurrentGPSData = new();
        LocationProvider?.OnNmeaRecieved += OnNmeaRecieved;
        LocationProvider?.OnNewLocation += OnNewLocation;
        LocationProvider?.Start(ntripOptions);
        LocationTimer = new((state) =>
        {
            OnLocationUpdated?.Invoke(CurrentGPSData.Copy());
        }, null, 1000, 1000);
    }

    public static void StopUpdatingLocation()
    {
        LocationTimer?.Dispose();
        LocationTimer = null;
        LocationProvider?.Stop();
        LocationProvider?.OnNmeaRecieved -= OnNmeaRecieved;
        LocationProvider?.OnNewLocation -= OnNewLocation;
        CurrentGPSData = null;
    }

    public static CancellationToken WaitForRequirements(Predicate<GPSData> checkRequirements)
    {
        if (CurrentGPSData is null || checkRequirements(CurrentGPSData))
        {
            return CancellationToken.None;
        }

        CancellationTokenSource source = new();
        void CancelToken(GPSData data)
        {
            if (checkRequirements(data))
            {
                OnLocationUpdated -= CancelToken;
                source.Cancel();
                source.Dispose();
            }
        }

        OnLocationUpdated += CancelToken;
        return source.Token;
    }

    public static CancellationToken WaitForRTKFix() => WaitForRequirements(data => data.Fix == Fix.RTKFix);
    public static CancellationToken WaitForEstimatedAccuracy(double requiredAccuracy) => WaitForRequirements(data => data.EstimatedAccuracy <= requiredAccuracy);
}

public enum ProviderType
{
    TCP,
    Bluetooth,
    TextFile,
    InternalGps,
    Map,
}

public static class Options
{
    // TCP Options
    public const string Host = "host";
    public const string Port = "port";

    // Bluetooth Options
    public const string COMPort = "comPort";
    public const string BaudRate = "baudRate";

    // TextFile Options
    public const string NmeaFile = "nmeaFile";

    // NTrip Options
    public const string NTripAddress = "ntripAddress";
    public const string NTripPort = "ntripPort";
    public const string NTripMountPoint = "ntripMountPoint";
    public const string NTripUsername = "ntripUsername";
    public const string NTripPassword = "ntripPassword";
}
