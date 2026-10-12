using GNSS.Nmea;
using Microsoft.Extensions.Logging;

namespace GNSS.Workflows;

/// <summary>
/// 
/// </summary>
/// <param name="waitingForRequirements">Called whenever the workflow needs to wait for incoming GPS data
/// to meet certain requirements. This method should handle any UI tasks that are needed durring this waiting period.
/// The supplied cancelation token gets canceled when all requirements for the incoming GPS data are met.</param>
public abstract class GNSSWorkflow(GPS gps, Func<CancellationToken, Task> waitingForRequirements)
{
    protected GPS GPS { get; } = gps;
    protected Func<CancellationToken, Task> WaitingForRequirementsAsync { get; } = waitingForRequirements;

    public bool RequireDGPS { get; set; }
    public bool RequireRTKFixed { get; set; }
    public bool RequireAcuracy { get; set; }
    public double RequiredAcuracy { get; set; }
    public bool RequireSatellites { get; set; }
    public int RequiredSatellites { get; set; }
    public bool RequireAge { get; set; }
    public int RequiredAge { get; set; }

    public string WaitingFor { get; protected set; } = string.Empty;

    protected virtual bool CheckRequirements(GPSData data)
    {
        FixQualityComparer fixComparer = new();

        if (RequireDGPS && fixComparer.Compare(data.FixQuality, FixQuality.DGPS) < 0)
        {
            WaitingFor = "Waiting for DGPS.";
            return false;
        }
        if (RequireRTKFixed && fixComparer.Compare(data.FixQuality, FixQuality.RTKFixed) < 0)
        {
            WaitingFor = "Waiting for RTK Fixed.";
            return false;
        }
        if (RequireAcuracy && data.EstimatedAccuracy < RequiredAcuracy)
        {
            WaitingFor = $"Waiting for estimated acuracy of {RequireAcuracy} meters.";
            return false;
        }
        if (RequireSatellites && data.SatellitesInUse < RequiredSatellites)
        {
            WaitingFor = $"Waiting for at least {RequiredSatellites} satellites.";
            return false;
        }
        if (RequireAge && data.Age < RequiredAge)
        {
            WaitingFor = $"Waiting for fix age less than {RequiredAge} seconds.";
            return false;
        }

        return true;
    }

    public abstract Task<GPSData> CollectVertexAsync();

    protected void LogCollection(GPSData data)
    {
        if (GPS.Logger?.IsEnabled(LogLevel.Information) ?? false)
        {
            GPS.Logger?.LogInformation("Collecting vertex at ({latitude}, {longitude}).", data.Latitude, data.Longitude);
        }
    }
}
