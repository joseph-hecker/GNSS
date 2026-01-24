using GNSS.Drivers;
using GNSS.Nmea;
using GNSS.Nmea.Exceptions;
using System.Diagnostics;

namespace GNSS;

public static class GPS
{
    private static IDriver? Driver { get; set; }
    private static GPSData? CurrentGPSData { get; set; }
    private static Timer? LocationTimer { get; set; }

    public delegate void OnLocationUpdatedEventHandler(GPSData data);
    public static event OnLocationUpdatedEventHandler? OnLocationUpdated;

    public static async Task<GPSData> GetNextLocationAsync()
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

    private static void OnNmeaRecieved(string nmea)
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

    public static void Connect(DriverType driverType, Dictionary<string, string> driverOptions)
    {
        Disconnect();
        Driver = driverType switch
        {
            DriverType.TCP => new TCPDriver(driverOptions[Options.Host], int.Parse(driverOptions[Options.Port])),
            DriverType.Bluetooth => new BluetoothDriver(driverOptions[Options.COMPort], int.Parse(driverOptions[Options.BaudRate])),
            DriverType.TextFile => new TextFileDriver(driverOptions[Options.NmeaFile]),

            _ => throw new InvalidOperationException($"'{driverType}' GPS driver is not supported."),
        };
    }

    public static void Disconnect()
    {
        StopUpdatingLocation();
        Driver = null;
    }

    public static void StartUpdatingLocation(Dictionary<string, string>? ntripOptions)
    {
        if (Driver is null)
        {
            throw new InvalidOperationException("GPS driver is not connected. Call Connect() first.");
        }

        CurrentGPSData = new();
        Driver.OnNmeaRecieved += OnNmeaRecieved;
        Driver.Start(ntripOptions);
        LocationTimer = new((state) =>
        {
            OnLocationUpdated?.Invoke(CurrentGPSData.Copy());
        }, null, 1000, 1000);
    }

    public static void StopUpdatingLocation()
    {
        LocationTimer?.Dispose();
        LocationTimer = null;
        Driver?.Stop();
        Driver?.OnNmeaRecieved -= OnNmeaRecieved;
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

public enum DriverType
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
