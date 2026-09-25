using GNSS.Providers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GNSS.Test.Providers;

public class NmeaService
{
    public static string Host => "127.0.0.1";
    public static int Port => 7776;
    public static Dictionary<string, string> Options => new()
    {
        { GNSS.Options.Host, Host },
        { GNSS.Options.Port, Port.ToString() }
    };

    private TextFileProvider TextFileDriver { get; set; }
    private TcpListener Listener { get; set; }
    private TcpClient? Client { get; set; }
    private StreamWriter? Writer { get; set; }


    public NmeaService(string nmeaFile)
    {
        TextFileDriver = new(nmeaFile);
        Listener = new(IPAddress.Loopback, Port);
        Listener.Start();
    }

    private void OnNmeaRevieved(string nmea)
    {
        if (nmea.StartsWith('$') && Writer is not null)
        {
            Writer.WriteAsync(nmea);
        }
    }

    public async Task StartSendingNmea()
    {
        TextFileDriver.Start();
        Client = await Listener.AcceptTcpClientAsync();
        NetworkStream stream = Client.GetStream();
        Writer = new(stream, Encoding.UTF8) { AutoFlush = true };
        TextFileDriver.OnNmeaRecieved += OnNmeaRevieved;
    }

    public void StopSendingNmea()
    {
        TextFileDriver.OnNmeaRecieved -= OnNmeaRevieved;
        TextFileDriver.Stop();
        Client?.Close();
        Client = null;
    }
}