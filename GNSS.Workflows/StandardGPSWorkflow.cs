namespace GNSS.Workflows;

public class StandardGPSWorkflow(Func<CancellationToken, Task> waitForRequirements) : GNSSWorkflow(waitForRequirements)
{
    public override async Task<GPSData> CollectVertexAsync()
    {
        CancellationToken ct = GPS.WaitForRequirements(CheckRequirements);
        await WaitForRequirementsAsync(ct);

        GPSData data = await GPS.GetNextLocationAsync();
        LogCollection(data);
        return data;
    }
}
