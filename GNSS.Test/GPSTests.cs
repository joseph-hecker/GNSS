namespace GNSS.Test;

public class GPSTests
{
    private static readonly Dictionary<ProviderType, Dictionary<string, string>> DriverOptions = new()
    {
        { ProviderType.TCP, new()
        {
            { Options.Host, "127.0.0.1" },
            { Options.Port, "77776" }
        }},
        { ProviderType.Bluetooth, new()
        {
            { Options.COMPort, "COM4" },
            { Options.BaudRate, "9600" }
        }},
        { ProviderType.TextFile, new()
        {
            { Options.NmeaFile, Util.GetNmeaFile("Arrow100_SBAS.txt") },
        }},
    };

    [TearDown]
    public void TearDown()
    {
        GPS.Disconnect();
    }

    [Test, TestOf(nameof(GPS.Connect))]
    public void TestConnect([Values] ProviderType driverType)
    {
        try
        {
            GPS.Connect(driverType, DriverOptions[driverType]);
        }
        catch (FormatException)
        {
            var driver = TestContext.CurrentContext.Test.Arguments[0];
            Assert.Fail($"{driver} driver options are invalid.");
        }
    }

    [Test, TestOf(nameof(GPS.StartUpdatingLocation))]
    public void TestStartUpdatingLocataion()
    {
        GPSData gpsData = new();
        ProviderType driverType = ProviderType.TextFile;
        GPS.Connect(driverType, DriverOptions[driverType]);
        GPS.StartUpdatingLocation();
        GPS.OnLocationUpdated += UpdateGPSData;
        Thread.Sleep(5_000);
        GPS.OnLocationUpdated -= UpdateGPSData;
        Assert.That(gpsData, Is.Not.EqualTo(new GPSData()));

        void UpdateGPSData(GPSData data)
        {
            Assert.That(data, Is.Not.EqualTo(gpsData));
            gpsData = data;
        }
    }

    [Test, TestOf(nameof(GPS.StartUpdatingLocation))]
    public void TestStartUpdatingLocataionWithNullDriver()
    {
        Assert.That(() => GPS.StartUpdatingLocation(), Throws.TypeOf<InvalidOperationException>());
    }

    [Test, TestOf(nameof(GPS.GetNextLocationAsync))]
    public async Task TestGetNextLocationAsync()
    {
        GPSData gpsData = new();
        ProviderType driverType = ProviderType.TextFile;
        GPS.Connect(driverType, DriverOptions[driverType]);
        GPS.StartUpdatingLocation();
        GPSData newGpsData = await GPS.GetNextLocationAsync();
        Assert.That(newGpsData, Is.Not.EqualTo(gpsData));
    }

    [Test, TestOf(nameof(GPS.WaitForRequirements))]
    public async Task TestWaitForRequirements()
    {
        ProviderType driverType = ProviderType.TextFile;
        GPS.Connect(driverType, DriverOptions[driverType]);
        GPS.StartUpdatingLocation();
        CancellationToken ct = GPS.WaitForRequirements(data => data.SatellitesInUse >= 10);
        while (!ct.WaitHandle.WaitOne()) { }
        GPSData data = await GPS.GetNextLocationAsync();
        Assert.That(data.SatellitesInUse, Is.GreaterThanOrEqualTo(10));
    }

    [Test, TestOf(nameof(GPS.WaitForRequirements))]
    public void TestWaitForRequirementsImmediateCancelation()
    {
        ProviderType driverType = ProviderType.TextFile;
        GPS.Connect(driverType, DriverOptions[driverType]);
        GPS.StartUpdatingLocation();
        CancellationToken ct = GPS.WaitForRequirements(data => true);
        Assert.That(ct, Is.EqualTo(CancellationToken.None));
    }
}
