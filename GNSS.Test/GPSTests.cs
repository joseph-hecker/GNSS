using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSS.Test;

public class GPSTests
{
    private static readonly Dictionary<DriverType, Dictionary<string, string>> DriverOptions = new()
    {
        { DriverType.TCP, new()
        {
            { Options.Host, "" },
            { Options.Port, "" }
        }},
        { DriverType.Bluetooth, new()
        {
            { Options.COMPort, "" },
            { Options.BaudRate, "" }
        }},
        { DriverType.TextFile, new()
        {
            { Options.NmeaFile, Path.Combine(Util.TestNmeaDirectory, "Arrow100_SBAS.txt") },
        }},
    };

    [TearDown]
    public void TearDown()
    {
        GPS.Disconnect();
    }

    [Test, TestOf(nameof(GPS.Connect))]
    public void TestConnect([Values] DriverType driverType)
    {
        try
        {
            GPS.Connect(driverType, DriverOptions[driverType]);
        }
        catch (FormatException)
        {
            var driver = TestContext.CurrentContext.Test.Arguments[0];
            Assert.Ignore($"{driver} driver options are invalid.");
        }
    }

    [Test, TestOf(nameof(GPS.StartUpdatingLocation))]
    public void TestStartUpdatingLocataion()
    {
        GPSData gpsData = new();
        DriverType driverType = DriverType.TextFile;
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
        DriverType driverType = DriverType.TextFile;
        GPS.Connect(driverType, DriverOptions[driverType]);
        GPS.StartUpdatingLocation();
        GPSData newGpsData = await GPS.GetNextLocationAsync();
        Assert.That(newGpsData, Is.Not.EqualTo(gpsData));
    }
}
