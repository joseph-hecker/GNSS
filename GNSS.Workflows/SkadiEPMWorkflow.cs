namespace GNSS.Workflows;

/// <summary>
/// GNSS workflow for Skadi Extensible Pole Mode.
/// </summary>
public class SkadiEPMWorkflow(Func<CancellationToken, Task> waitForRequirements) : SkadiTiltCompWorkflow(waitForRequirements)
{
    public override async Task<GPSData> CollectVertexAsync()
    {
        CancellationToken ct = GPS.WaitForRequirements(CheckRequirements);
        await WaitForRequirementsAsync(ct);

        ct = GPS.WaitForSkadiTriggerPressed();
        TaskCompletionSource<GPSData> source = new();
        var registration = ct.UnsafeRegister(async (state, ct) =>
        {
            source.SetResult(await GPS.GetNextLocationAsync());
        }, null);

        GPSData data = await source.Task;
        registration.Unregister();

        // Check that all requirements are still met after waiting for a trigger press.
        if (!CheckRequirements(data))
        {
            return await CollectVertexAsync();
        }

        LogCollection(data);
        return data;
    }
}
