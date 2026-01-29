using GNSS.Nmea;
using System.Text;

namespace GNSS.Test;

public static class Util
{
    private const string INVALID_NMEA_FILE_KEYWORD = "INVALID";

    public static string TestDataDirectory
    {
        get
        {
            string projectDirectory = Directory.GetParent(TestContext.CurrentContext.TestDirectory)?.Parent?.Parent?.FullName
                ?? throw new DirectoryNotFoundException("Could not find project directory.");
            return Path.Combine(projectDirectory, "TestData");
        }
    }

    public static string TestNmeaDirectory => Path.Combine(TestDataDirectory, "Nmea");

    public static string GetNmeaFile(string nmeaFile)
    {
        return Path.Combine(TestNmeaDirectory, nmeaFile);
    }

    /// <summary>
    /// Retrieves the list of valid NMEA test files from the test directory, ie. files that contain GGA messages.
    /// </summary>
    /// <returns>An array of file paths for all files in the test directory that do not contain the
    /// string "INVALID" in their file names.</returns>
    public static string[] GetValidNmeaFiles()
    {
        IEnumerable<string> txtFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.txt").Where(file => !file.Contains(INVALID_NMEA_FILE_KEYWORD));
        IEnumerable<string> logFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.log").Where(file => !file.Contains(INVALID_NMEA_FILE_KEYWORD));
        return [.. txtFiles, .. logFiles];
    }

    /// <summary>
    /// Retrieves the list of invalid NMEA test files from the test directory, ie. files that do not contain GGA messages.
    /// </summary>
    /// <returns>An array of file paths for all files in the test directory that contain the
    /// string "INVALID" in their file names.</returns>
    public static string[] GetInvalidNmeaFiles()
    {
        IEnumerable<string> txtFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.txt").Where(file => file.Contains(INVALID_NMEA_FILE_KEYWORD));
        IEnumerable<string> logFiles = Directory.GetFiles(Util.TestNmeaDirectory, "*.log").Where(file => file.Contains(INVALID_NMEA_FILE_KEYWORD));
        return [.. txtFiles, .. logFiles];
    }

    public static string GenerateChecksum(string message)
    {
        if (message.StartsWith('$'))
        {
            message = message[1..];
        }

        byte checksum = Encoding.ASCII.GetBytes(message).Aggregate((checksum, current) => (byte)(checksum ^ current));
        return checksum.ToString("X");
    }

    public class Tests
    {
        [Test, TestOf(nameof(GenerateChecksum))]
        [TestCase("$GPGGA,172814.00,3723.46587704,N,12202.26957864,W,2,6,1.2,18.893,M,-25.669,M,2.0,0031")]
        public void TestGenerateChecksum(string message)
        {
            Assert.That(() => NmeaSentence.Parse(message + $"*{GenerateChecksum(message)}"), Throws.Nothing);
        }
    }
}
