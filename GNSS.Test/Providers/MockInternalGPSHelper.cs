using GNSS.Providers;

namespace GNSS.Test.Providers;

public class MockInternalGPSHelper : IInternalGPSHelper
{
    public async Task<bool> EnsurePermissionsAsync(CancellationToken ct = default) => true;

    public void Start(Action<GPSLocation> onNewLocation) { }

    public void Stop() { }
}
