using System.Net.Sockets;
using System.Text;

namespace GNSS.Drivers;

public class TCPDriver : IDriver
{
    public event IDriver.OnNmeaRecievedEventHandler? OnNmeaRecieved;

    public double PoleHeight { get; set; }

    private string Host { get; set; }
    private int Port { get; set; }
    private TcpClient? Client { get; set; }

    public TCPDriver(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public void Start(Dictionary<string, string>? ntripOptions)
    {
        Task.Run(() =>
        {
            Client = new(Host, Port);
            NetworkStream stream = Client.GetStream();
            StreamReader reader = new(stream, Encoding.UTF8);
            while (Client.Connected)
            {
                if (reader.Peek() != -1)
                {
                    string? data = reader.ReadLine();
                    if (data is not null && data.StartsWith('$'))
                    {
                        OnNmeaRecieved?.Invoke(data);
                    }
                }
            }
        });
    }

    public void Stop()
    {
        Client?.Close();
        Client = null;
    }
}
