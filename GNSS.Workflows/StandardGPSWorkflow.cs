namespace GNSS.Workflows;

public class StandardGPSWorkflow(Func<CancellationToken, Task> waitingForRequirements) : GNSSWorkflow(waitingForRequirements)
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
