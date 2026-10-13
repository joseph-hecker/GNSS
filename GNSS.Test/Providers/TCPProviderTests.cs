using GNSS.Providers;

namespace GNSS.Test.Providers;

public class TCPProviderTests
{
    private NmeaService Service { get; } = new(Util.GetNmeaFile("Arrow100_SBAS.txt"));

    [SetUp]
    public void SetUp()
    {
        Task.Run(async () => await Service.StartSendingNmea());
    }

    [TearDown]
    public void TearDown()
    {
        Service.StopSendingNmea();
    }

    [Test, TestOf(nameof(TCPProvider.Start))]
    public void TestStart()
    {
        List<string> messages = [];
        TCPProvider driver = new(NmeaService.Host, NmeaService.Port);
        driver.OnNewGPSData += (sender, data) => { if (data is string nmea) messages.Add(nmea); };
        driver.Start();
        Thread.Sleep(5_000);
        driver.Stop();
        Assert.That(messages, Is.Not.Empty);
        TestContext.Out.WriteLine($"Received {messages.Count} NMEA messages.");
    }
}
