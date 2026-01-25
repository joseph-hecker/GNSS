using GNSS.Drivers;

namespace GNSS.Test.Drivers;

public class TextFileDriverTests
{
    [Test, TestOf(nameof(TextFileDriver.Start))]
    [TestCaseSource(typeof(Util), nameof(Util.GetValidNmeaFiles))]
    public void TestValidStart(string nmeaFile)
    {
        List<string> messages = [];
        TextFileDriver driver = new(nmeaFile);
        driver.OnNmeaRecieved += messages.Add;
        driver.Start();
        Thread.Sleep(5_000);
        driver.Stop();
        Assert.That(messages, Is.Not.Empty);
        TestContext.Out.WriteLine($"Received {messages.Count} NMEA messages.");
    }

    [Test, TestOf(nameof(TextFileDriver.Start))]
    [TestCaseSource(typeof(Util), nameof(Util.GetInvalidNmeaFiles))]
    public void TestInvalidStart(string nmeaFile)
    {
        List<string> messages = [];
        Assert.Throws<InvalidOperationException>(() => new TextFileDriver(nmeaFile));
    }
}
