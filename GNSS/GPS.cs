using GNSS.Providers;
using GNSS.Nmea;
using GNSS.Nmea.Exceptions;
using System.Diagnostics;

namespace GNSS;

public class GPS
{
    private ILocationProvider? LocationProvider { get; set; }
    private GPSData? CurrentGPSData { get; set; }
    private Timer? LocationTimer { get; set; }

    public delegate void OnLocationUpdatedEventHandler(GPSData data);
    public event OnLocationUpdatedEventHandler? OnLocationUpdated;

    public async Task<GPSData> GetNextLocationAsync()
    {
        TaskCompletionSource<GPSData> source = new();
        void SetResult(GPSData data)
        {
            OnLocationUpdated -= SetResult;
            source.SetResult(data);
        }

        OnLocationUpdated += SetResult;
        return await source.Task;
    }

    private void OnNmeaRecieved(string nmea)
    {
        try
        {
            NmeaSentence? sentence = NmeaSentence.Parse(nmea);
            if (CurrentGPSData is not null)
            {
                sentence?.UpdateGPSData(CurrentGPSData);
            }
        }
        catch (ChecksumException) { }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public void Connect(ProviderType driverType, Dictionary<string, string> driverOptions)
    {
        Disconnect();
        LocationProvider = driverType switch
        {
            ProviderType.TCP => new TCPProvider(driverOptions[Options.Host], int.Parse(driverOptions[Options.Port])),
            ProviderType.Bluetooth => new BluetoothProvider(driverOptions[Options.COMPort], int.Parse(driverOptions[Options.BaudRate])),
            ProviderType.TextFile => new TextFileProvider(driverOptions[Options.NmeaFile]),

            _ => throw new InvalidOperationException($"'{driverType}' GPS driver is not supported."),
        };
    }

    public void Disconnect()
    {
        StopUpdatingLocation();
        LocationProvider = null;
    }

    public void StartUpdatingLocation(Dictionary<string, string>? ntripOptions = null)
    {
        if (LocationProvider is null)
        {
            throw new InvalidOperationException("GPS driver is not connected. Call Connect() first.");
        }

        CurrentGPSData = new();
        LocationProvider.OnNmeaRecieved += OnNmeaRecieved;
        LocationProvider.Start(ntripOptions);
        LocationTimer = new((state) =>
        {
            OnLocationUpdated?.Invoke(CurrentGPSData.Copy());
        }, null, 1000, 1000);
    }

    public void StopUpdatingLocation()
    {
        LocationTimer?.Dispose();
        LocationTimer = null;
        LocationProvider?.Stop();
        LocationProvider?.OnNmeaRecieved -= OnNmeaRecieved;
        CurrentGPSData = null;
    }

    public CancellationToken WaitForRequirements(Predicate<GPSData> checkRequirements)
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

    public CancellationToken WaitForRTKFix() => WaitForRequirements(data => data.Fix == Fix.RTKFix);
    public CancellationToken WaitForEstimatedAccuracy(double requiredAccuracy) => WaitForRequirements(data => data.EstimatedAccuracy <= requiredAccuracy);
}

public enum ProviderType
{
    TCP,
    Bluetooth,
    TextFile,
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
