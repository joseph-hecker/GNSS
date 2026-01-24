using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSS.Test;

public static class Util
{
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
}
