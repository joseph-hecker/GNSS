using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NmeaService;

public class NmeaService
{
    public static string Host => "127.0.0.1";
    public static int Port => 6789;

    private FileBuffer FileBuffer { get; set; }
    private TcpListener Listener { get; set; }
    private TcpClient? Client { get; set; }
    private Timer? Timer { get; set; }


    public NmeaService(string nmeaFile)
    {
        FileBuffer = new(nmeaFile);
        Listener = new(IPAddress.Loopback, Port);
        Listener.Start();
    }

    public void StartSendingNmea()
    {
        Task.Run(async () =>
        {
            StreamWriter? writer = null;
            Timer timer = new(async state =>
            {
                try
                {
                    string[] chunk = FileBuffer.ComsumeNextChunk();
                    foreach (string line in chunk)
                    {
                        Console.Write(line);
                        if (line.StartsWith('$') && writer is not null)
                        {
                            await writer.WriteAsync(line);
                        }
                    }
                }
                catch (IOException)
                {
                    Client = await Listener.AcceptTcpClientAsync();
                    NetworkStream stream = Client.GetStream();
                    writer = new(stream, Encoding.UTF8) { AutoFlush = true };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }, null, 0, 1000);

            Client = await Listener.AcceptTcpClientAsync();
            NetworkStream stream = Client.GetStream();
            writer = new(stream, Encoding.UTF8) { AutoFlush = true };
        });
    }

    public void StopSendingNmea()
    {
        Timer?.Dispose();
        Timer = null;
        Client?.Close();
        Client = null;
    }
}
