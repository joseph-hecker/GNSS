using GNSS.Providers;

namespace GNSS.Test.Providers;

public class MockMapHelper : IMapHelper
{
    public void Start(Action<GPSLocation> onNewLocation) { }

    public void Stop() { }
}
