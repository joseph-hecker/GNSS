using GNSS.Providers;

namespace GNSS.Test.Drivers;

public class TextFileDriverTests
{
    [Test, TestOf(nameof(TextFileProvider.Start))]
    [TestCaseSource(typeof(Util), nameof(Util.GetValidNmeaFiles))]
    public void TestValidStart(string nmeaFile)
    {
        List<string> messages = [];
        TextFileProvider driver = new(nmeaFile);
        driver.OnNmeaRecieved += messages.Add;
        driver.Start();
        Thread.Sleep(5_000);
        driver.Stop();
        Assert.That(messages, Is.Not.Empty);
        TestContext.Out.WriteLine($"Received {messages.Count} NMEA messages.");
    }

    [Test, TestOf(nameof(TextFileProvider.Start))]
    [TestCaseSource(typeof(Util), nameof(Util.GetInvalidNmeaFiles))]
    public void TestInvalidStart(string nmeaFile)
    {
        List<string> messages = [];
        Assert.Throws<InvalidOperationException>(() => new TextFileProvider(nmeaFile));
    }
}
