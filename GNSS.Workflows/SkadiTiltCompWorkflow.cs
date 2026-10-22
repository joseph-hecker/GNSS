namespace GNSS.Workflows;

/// <summary>
/// GNSS workflow for Skadi Tilt Compensation Mode and Skadi Invisible Pole Mode.
/// </summary>
public class SkadiTiltCompWorkflow(Func<CancellationToken, Task> waitForRequirements) : GNSSWorkflow(waitForRequirements)
{
    public new bool RequireDGPS => true;
    public new bool RequireRTKFixed => true;

    protected override bool CheckRequirements(GPSData data)
    {
        ArgumentNullException.ThrowIfNull(data.EnavData, nameof(data.EnavData));
        if (base.CheckRequirements(data))
        {
            if (data.EnavData.TiltCompInitilized)
            {
                // We already have RTK Fixed so we just need to calibrate the IMU sensors
                WaitingFor = "Waiting for tilt compensation to be initialized. Please shake the Skadi to calibrate it.";
                return false;
            }
        }

        return true;
    }

    public override async Task<GPSData> CollectVertexAsync()
    {
        CancellationToken ct = GPS.WaitForRequirements(CheckRequirements);
        await WaitForRequirementsAsync(ct);

        GPSData data = await GPS.GetNextLocationAsync();
        LogCollection(data);
        return data;
    }
}
