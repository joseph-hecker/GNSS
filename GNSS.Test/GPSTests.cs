using GNSS.Test.Providers;

namespace GNSS.Test;

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

    private GPS GPS { get; } = new();

    private void StartGPSWithTextFileProvider()
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
        try
        {
            if (providerType  == ProviderType.InternalGPS)
            {
                MockInternalGPSHelper helper = new();
                Assert.That(() => GPS.Connect(helper), Throws.Nothing);
            }
            else if (providerType == ProviderType.Map)
            {
                MockMapHelper helper = new();
                Assert.That(() => GPS.Connect(helper), Throws.Nothing);
            }
            else
            {
                Assert.That(() => GPS.Connect(providerType, ProviderOptions[providerType]), Throws.Nothing);
            }
        }
        catch (FormatException)
        {
            var provider = TestContext.CurrentContext.Test.Arguments[0];
            Assert.Fail($"{provider} provider options are invalid.");
        }
    }

    [Test, TestOf(nameof(GPS.StartUpdatingLocation))]
    public void TestStartUpdatingLocataion()
    {
        GPSData gpsData = new();
        void UpdateGPSData(GPSData data)
        {
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
