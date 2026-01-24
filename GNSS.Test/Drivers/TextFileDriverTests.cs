using GNSS.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSS.Test.Drivers;

public class TextFileDriverTests
{
    private const string INVALID_NMEA_FILE_KEYWORD = "INVALID";

    /// <summary>
    /// Retrieves the list of valid NMEA test files from the test directory, ie. files that contain GGA messages.
    /// </summary>
    /// <returns>An array of file paths for all files in the test directory that do not contain the
    /// string "INVALID" in their file names.</returns>
    private static string[] GetValidNmeaFiles()
    {
        IEnumerable<string> txtFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.txt").Where(file => !file.Contains(INVALID_NMEA_FILE_KEYWORD));
        IEnumerable<string> logFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.log").Where(file => !file.Contains(INVALID_NMEA_FILE_KEYWORD));
        return [..txtFiles, ..logFiles];
    }

    /// <summary>
    /// Retrieves the list of invalid NMEA test files from the test directory, ie. files that do not contain GGA messages.
    /// </summary>
    /// <returns>An array of file paths for all files in the test directory that contain the
    /// string "INVALID" in their file names.</returns>
    private static string[] GetInvalidNmeaFiles()
    {
        IEnumerable<string> txtFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.txt").Where(file => file.Contains(INVALID_NMEA_FILE_KEYWORD));
        IEnumerable<string> logFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.log").Where(file => file.Contains(INVALID_NMEA_FILE_KEYWORD));
        return [..txtFiles, ..logFiles];
    }

    [Test, TestOf(nameof(TextFileDriver.Start))]
    [TestCaseSource(nameof(GetValidNmeaFiles))]
    public void TestValidStart(string nmeaFile)
    {
        List<string> messages = [];
        TextFileDriver driver = new(nmeaFile);
        driver.OnNmeaRecieved += messages.Add;
        driver.Start();
        Thread.Sleep(5_000);
        driver.Stop();
        Assert.That(messages, Is.Not.Empty);
    }

    [Test, TestOf(nameof(TextFileDriver.Start))]
    [TestCaseSource(nameof(GetInvalidNmeaFiles))]
    public void TestInvalidStart(string nmeaFile)
    {
        List<string> messages = [];
        Assert.That(() => new TextFileDriver(nmeaFile), Throws.TypeOf<InvalidOperationException>());
    }
}
