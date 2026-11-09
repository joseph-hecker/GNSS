namespace GNSS.Workflows;

public interface IMapCenterSource
{
    public GPSLocation GetMapCenter();
}

public class MapWorkflow(IMapCenterSource map) : GNSSWorkflow((ct) => Task.CompletedTask)
{
    public new bool RequireDGPS => false;
    public new bool RequireRTKFixed => false;
    public new bool RequireAcuracy => false;
    public new bool RequireSatellites => false;
    public new bool RequireAge => false;

    private IMapCenterSource Map { get; } = map;

    protected override bool CheckRequirements(GPSData data) => true;

    public override Task<GPSData> CollectVertexAsync()
    {
        GPSData gpsData = new(Map.GetMapCenter());
        return Task.FromResult(gpsData);
    }
}
