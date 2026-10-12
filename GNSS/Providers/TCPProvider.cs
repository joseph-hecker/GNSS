using System.Net.Sockets;
using System.Text;

namespace GNSS.Providers;

public class TCPProvider : ILocationProvider
{
    public event EventHandler<object>? OnNewGPSData;

    private string Host { get; set; }
    private int Port { get; set; }
    private TcpClient? Client { get; set; }

    public TCPProvider(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public void Start(Dictionary<string, string>? ntripOptions = null)
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
                        OnNewGPSData?.Invoke(this, data);
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
