using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSS.Drivers;

public class TextFileDriver : IDriver
{
    public event IDriver.OnNmeaRecievedEventHandler? OnNmeaRecieved;

    public double PoleHeight { get; set; }

    private FileStream Stream { get; set; }
    private Timer? NmeaTimer { get; set; }

    public TextFileDriver(string nmeaFile)
    {
        Stream = new(nmeaFile, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private void JumpToNextNmeaMessage()
    {
        int c = Stream.ReadByte();
        while (c != -1 || c != '$')
        {
            c = Stream.ReadByte();
        }

        if (c == -1)
        {
            Stream.Seek(0, SeekOrigin.Begin);
            JumpToNextNmeaMessage();
        }
        else if (c == '$')
        {
            Stream.Position--;
        }
    }

    private string ReadNextNmeaMessage()
    {
        JumpToNextNmeaMessage();
        StringBuilder sb = new();
        int c = Stream.ReadByte();
        while (c != -1)
        {
            sb.Append((char)c);
            if (c == '\n')
            {
                break;
            }

            c = Stream.ReadByte();
        }

        return sb.ToString();
    }

    private bool PeekGGA()
    {
        long start = Stream.Position;
        JumpToNextNmeaMessage();
        StringBuilder sb = new();
        int c = Stream.ReadByte();
        while (c != ',')
        {
            sb.Append((char)c);
            c = Stream.ReadByte();
        }

        Stream.Seek(start, SeekOrigin.Begin);
        string messageID = sb.ToString();
        return messageID.Contains("GGA");
    }

    private string[] ConsumeNextChunk()
    {
        List<string> chunk = [ReadNextNmeaMessage()];
        while (!PeekGGA())
        {
            chunk.Add(ReadNextNmeaMessage());
        }

        return [.. chunk];
    }

    public void Start(Dictionary<string, string>? ntripOptions)
    {
        NmeaTimer = new((state) =>
        {
            string[] nmeaMessages = ConsumeNextChunk();
            foreach (var message in nmeaMessages)
            {
                OnNmeaRecieved?.Invoke(message);
            }
        }, null, 0, 1000);
    }

    public void Stop()
    {
        NmeaTimer?.Dispose();
        NmeaTimer = null;
    }
}
