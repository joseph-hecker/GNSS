using GNSS.Drivers;

namespace GNSS.Test.Drivers;

public class TCPDriverTests
{
    [Test, TestOf(nameof(TCPDriver.Start))]
    public void TestStart()
    {
        NmeaService service = new(Util.GetNmeaFile("Arrow100_SBAS.txt"));
        Task.Run(async () =>
        {
            await service.StartSendingNmea();
        });

        List<string> messages = [];
        TCPDriver driver = new(NmeaService.Host, NmeaService.Port);
        driver.OnNmeaRecieved += messages.Add;
        driver.Start();
        Thread.Sleep(5_000);
        driver.Stop();
        service.StopSendingNmea();
        Assert.That(messages, Is.Not.Empty);
        TestContext.Out.WriteLine($"Received {messages.Count} NMEA messages.");
    }
}
