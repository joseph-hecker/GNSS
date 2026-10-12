using GNSS.Nmea;
using Microsoft.Extensions.Logging;

namespace GNSS.Workflows;

public abstract class GNSSWorkflow(Func<CancellationToken, Task> waitingForRequirements)
{
    protected Func<CancellationToken, Task> WaitingForRequirementsAsync { get; } = waitingForRequirements;

    public bool RequireDGPS { get; set; }
    public bool RequireRTKFix { get; set; }
    public bool RequireAcuracy { get; set; }
    public double RequiredAcuracy { get; set; }
    public bool RequireSatellites { get; set; }
    public int RequiredSatellites { get; set; }
    public bool RequireAge { get; set; }
    public int RequiredAge { get; set; }

    protected virtual bool CheckRequirements(GPSData data)
    {
        if (RequireDGPS && data.FixQuality < FixQuality.DGPS) return false;
        if (RequireRTKFix && data.FixQuality != FixQuality.RTKFix) return false;
        if (RequireAcuracy && data.EstimatedAccuracy < RequiredAcuracy) return false;
        if (RequireSatellites && data.SatellitesInUse < RequiredSatellites) return false;
        if (RequireAge && data.Age < RequiredAge) return false;

        return true;
    }

    public abstract Task<GPSData> CollectVertexAsync();

    protected static void LogCollection(GPSData data)
    {
        if (GPS.Logger?.IsEnabled(LogLevel.Information) ?? false)
        {
            GPS.Logger?.LogInformation("Collecting vertex at ({latitude}, {longitude}).", data.Latitude, data.Longitude);
        }
    }
}
