using GNSS.Providers;

namespace GNSS.Test.Providers;

public class TextFileProviderTests
{
    [Test, TestOf(nameof(TextFileProvider.Start))]
    [TestCaseSource(typeof(Util), nameof(Util.GetValidNmeaFiles))]
    public void TestValidStart(string nmeaFile)
    {
        List<string> messages = [];
        TextFileProvider driver = new(nmeaFile);
        driver.OnNewGPSData += (sender, data) => { if (data is string nmea) messages.Add(nmea); };
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
