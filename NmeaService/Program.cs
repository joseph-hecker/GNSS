namespace NmeaService;

public static class Program
{
    private static NmeaService? Service { get; set; }

    public static void Main()
    {
        var documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        var nmeaFile = Path.Combine(documentsDir, "nmea", "Arrow100_SBAS.txt");
        StartService(nmeaFile);
        Console.ReadKey();
    }

    private static void StopServiceOnCancel(object? sender, EventArgs e)
    {
        Service?.StopSendingNmea();
    }

    private static void StartService(string filePath)
    {
        Service = new(filePath);
        Service.StartSendingNmea();
        Console.CancelKeyPress += StopServiceOnCancel;
    }
}