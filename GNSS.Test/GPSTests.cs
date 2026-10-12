namespace GNSS.Test;

[TestFixture, TestOf(nameof(GPS))]
public class GPSTests
{
    private static readonly Dictionary<ProviderType, Dictionary<string, string>> ProviderOptions = new()
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

    private static void StartGPSWithTextFileProvider()
    {
        ProviderType providerType = ProviderType.TextFile;
        GPS.Connect(providerType, ProviderOptions[providerType]);
        GPS.StartUpdatingLocation();
    }

    [TearDown]
    public void TearDown()
    {
        GPS.Disconnect();
    }

    [Test, TestOf(nameof(GPS.Connect))]
    public void TestConnect([Values] ProviderType providerType)
    {
        if (providerType == ProviderType.InternalGPS ||
            providerType == ProviderType.Map)
        {
            Assert.Ignore();
        }

        try
        {
            GPS.Connect(providerType, ProviderOptions[providerType]);
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
        void UpdateGPSData(GPSData data)
        {
            Assert.That(data, Is.Not.EqualTo(gpsData));
            gpsData = data;
        }

        StartGPSWithTextFileProvider();
        GPS.OnLocationUpdated += UpdateGPSData;
        Thread.Sleep(5_000);
        GPS.OnLocationUpdated -= UpdateGPSData;
        Assert.That(gpsData, Is.Not.EqualTo(new GPSData()));
    }

    [Test, TestOf(nameof(GPS.StartUpdatingLocation))]
    public void TestStartUpdatingLocataionWithNullDriver()
    {
        Assert.That(() => GPS.StartUpdatingLocation(), Throws.TypeOf<InvalidOperationException>());
    }

    [Test, TestOf(nameof(GPS.GetNextLocationAsync))]
    public async Task TestGetNextLocationAsync()
    {
        Assert.ThrowsAsync<InvalidOperationException>(GPS.GetNextLocationAsync);

        StartGPSWithTextFileProvider();
        Thread.Sleep(5_000);
        GPSData gpsData = await GPS.GetNextLocationAsync();
        Assert.That(gpsData, Is.Not.EqualTo(new GPSData()));
    }

    [Test, TestOf(nameof(GPS.WaitForRequirements))]
    public async Task TestWaitForRequirements()
    {
        StartGPSWithTextFileProvider();
        CancellationToken ct = GPS.WaitForRequirements(data => data.SatellitesInUse >= 10);
        while (!ct.WaitHandle.WaitOne()) { }
        GPSData data = await GPS.GetNextLocationAsync();
        Assert.That(data.SatellitesInUse, Is.GreaterThanOrEqualTo(10));
    }

    [Test, TestOf(nameof(GPS.WaitForRequirements))]
    public void TestWaitForRequirementsImmediateCancelation()
    {
        StartGPSWithTextFileProvider();
        CancellationToken ct = GPS.WaitForRequirements(data => true);
        Assert.That(ct, Is.EqualTo(CancellationToken.None));
    }
}
