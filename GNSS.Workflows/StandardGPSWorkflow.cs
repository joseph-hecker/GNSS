namespace GNSS.Workflows;

public class StandardGPSWorkflow(GPS gps, Func<CancellationToken, Task> waitingForRequirements) : GNSSWorkflow(gps, waitingForRequirements)
{
    public override async Task<GPSData> CollectVertexAsync()
    {
        CancellationToken ct = GPS.WaitForRequirements(CheckRequirements);
        await WaitingForRequirementsAsync(ct);

        GPSData data = await GPS.GetNextLocationAsync();
        LogCollection(data);
        return data;
    }
}
