namespace GNSS.Workflows;

public class RangeRangeWorkflow(Func<CancellationToken, Task> waitForRequirements) : GNSSWorkflow(waitForRequirements)
{
    public override Task<GPSData> CollectVertexAsync()
    {
        throw new NotImplementedException();
    }
}
