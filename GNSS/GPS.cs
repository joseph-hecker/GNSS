using GNSS.Nmea;
using GNSS.Nmea.Exceptions;
using GNSS.Providers;
using Microsoft.Extensions.Logging;

namespace GNSS;

public class GPS(ILogger<GPS>? logger = null)
{
    private ILocationProvider? LocationProvider { get; set; }
    private GPSData? CurrentGPSData { get; set; }
    private Timer? LocationTimer { get; set; }
    private GSVAccumulator? GSVAccumulator { get; set; }

    public delegate void OnLocationUpdatedEventHandler(GPSData data);
    public event OnLocationUpdatedEventHandler? OnLocationUpdated;

    public ILogger? Logger { get; set; } = logger;

    public async Task<GPSData> GetNextLocationAsync()
    {
        if (CurrentGPSData is null)
        {
            throw new InvalidOperationException("GPS is not streaming data.");
        }

        TaskCompletionSource<GPSData> source = new();
        void SetData(GPSData data)
        {
            OnLocationUpdated -= SetData;
            source.SetResult(data);
        }

        OnLocationUpdated += SetData;
        return await source.Task;
    }

    /// <summary>
    /// Called when an <see cref="ILocationProvider"/> recieves a new piece of GPS data.
    /// </summary>
    /// <param name="sender">The <see cref="ILocationProvider"/> that recieved the GPS data.</param>
    /// <param name="data">The nmea <see langword="string"/> or <see cref="GPSLocation"/> with the GPS data.</param>
    private void OnNewGPSData(object? sender, object data)
    {
        if (data is string nmea)
        {
            OnNmeaRecieved(nmea);
        }
        else if (data is GPSLocation location)
        {
            OnNewLocation(location);
        }
        else
        {
            Logger?.LogWarning("Could not read GPS data of type `{dataType}`.", data.GetType());
        }
    }

    private void OnNmeaRecieved(string nmea)
    {
        try
        {
            NmeaSentence? sentence = NmeaSentence.Parse(nmea);
            if (sentence is not null && CurrentGPSData is not null)
            {
                if (sentence is GSV gsv)
                {
                    GSVAccumulator?.Apply(gsv, CurrentGPSData);
                }
                else
                {
                    sentence.UpdateGPSData(CurrentGPSData); ;
                }

                Logger?.LogInformation("{nmea}", sentence.ToString());
            }
        }
        catch (ChecksumException) { }
        catch (NmeaException ex)
        {
            Logger?.LogWarning(ex, "{message} {nmea}", ex.Message, ex.Nmea);
        }
        catch (Exception ex)
        {
            Logger?.LogWarning(ex, "{message}", ex.Message);
        }
    }

    private void OnNewLocation(GPSLocation location)
    {
        if (CurrentGPSData is not null)
        {
            location.UpdateGPSData(CurrentGPSData);
            OnLocationUpdated?.Invoke(CurrentGPSData.Copy());
        }
    }

    private void Connect(ILocationProvider provider)
    {
        Disconnect();
        LocationProvider = provider;
        Logger?.LogInformation("GPS connected to {provider} provider.", provider.GetType().Name);
    }

    public void Connect(ProviderType providerType, Dictionary<string, string> providerOptions)
    {
        Connect(providerType switch
        {
            ProviderType.TCP => new TCPProvider(providerOptions[Options.Host], int.Parse(providerOptions[Options.Port])),
            ProviderType.Bluetooth => new BluetoothProvider(providerOptions[Options.COMPort], int.Parse(providerOptions[Options.BaudRate])),
            ProviderType.TextFile => new TextFileProvider(providerOptions[Options.NmeaFile]),
            ProviderType.InternalGPS => throw new InvalidOperationException("Use `Connect(IInternalGPSHelper helper)` instead."),
            ProviderType.Map => throw new InvalidOperationException("Use `Connect(IMapHelper helper)` instead."),
            _ => throw new InvalidOperationException($"`{providerType}` GPS provider is not supported."),
        });
    }

    public void Connect(IInternalGPSHelper helper) => Connect(new InternalGPSProvider(helper));

    public void Connect(IMapHelper helper) => Connect(new MapProvider(helper));

    public void Disconnect()
    {
        StopUpdatingLocation();
        LocationProvider = null;
    }

    public void StartUpdatingLocation(Dictionary<string, string>? ntripOptions = null)
    {
        if (LocationProvider is null)
        {
            throw new InvalidOperationException("GPS is not connected.");
        }

        CurrentGPSData = new();
        GSVAccumulator = new();
        LocationProvider?.OnNewGPSData += OnNewGPSData;
        LocationProvider?.Start(ntripOptions);
        LocationTimer = new((state) =>
        {
            if (CurrentGPSData is not null)
            {
                OnLocationUpdated?.Invoke(CurrentGPSData.Copy());
            }
        }, null, 1000, 1000);
    }

    public void StopUpdatingLocation()
    {
        LocationTimer?.Dispose();
        LocationTimer = null;
        LocationProvider?.OnNewGPSData -= OnNewGPSData;
        LocationProvider?.Stop();
        GSVAccumulator = null;
        CurrentGPSData = null;
    }

    public CancellationToken WaitForRequirements(Predicate<GPSData> checkRequirements)
    {
        if (CurrentGPSData is null || LocationProvider is MapProvider || checkRequirements(CurrentGPSData))
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

    public CancellationToken WaitForRTKFixed() => WaitForRequirements(data => data.FixQuality == FixQuality.RTKFixed);
    public CancellationToken WaitForEstimatedAccuracy(double requiredAccuracy) => WaitForRequirements(data => data.EstimatedAccuracy <= requiredAccuracy);
    public CancellationToken WaitForSkadiTriggerPressed() => WaitForRequirements(data => data.EnavData?.TriggerIsPressed ?? throw new InvalidOperationException("There was no enav data to check."));
}

public enum ProviderType
{
    TCP,
    Bluetooth,
    TextFile,
    InternalGPS,
    Map,
}

public static class Options
{
    public const string Provider = "provider";

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
