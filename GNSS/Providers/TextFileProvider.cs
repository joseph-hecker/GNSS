using System.Text;

namespace GNSS.Providers;

public class TextFileProvider : ILocationProvider
{
    public event EventHandler<object>? OnNewGPSData;

    private FileStream Stream { get; set; }
    private Timer? NmeaTimer { get; set; }

    public TextFileProvider(string nmeaFile)
    {
        Stream = new(nmeaFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (!SearchGGA())
        {
            throw new InvalidOperationException("Provided nmea file must have GGA messages for Text File provider to function.");
        }
    }

    /// <summary>
    /// Checks if the file contains any GGA messages.
    /// </summary>
    /// <returns><see langword="true"/> if there is at least one GGA message in the file; otherwise, <see langword="false"/>.</returns>
    private bool SearchGGA()
    {
        long start = Stream.Position;
        Stream.Seek(0, SeekOrigin.Begin);
        string messageID = string.Empty;
        while (!messageID.Contains("GGA"))
        {
            int c = Stream.ReadByte();
            while (c != '$')
            {
                if (c == -1)
                {
                    return false;
                }

                c = Stream.ReadByte();
            }

            byte[] buffer = new byte[6];
            Stream.ReadExactly(buffer);
            messageID = Encoding.ASCII.GetString(buffer);
        }

        Stream.Seek(start, SeekOrigin.Begin);
        return messageID.Contains("GGA");
    }

    /// <summary>
    /// Advances the stream position to the start of the next NMEA message, identified by a '$' character.
    /// </summary>
    /// <remarks>If the end of the stream is reached without finding a '$' character, the stream position is reset
    /// to the beginning and the search continues from the start.</remarks>
    private void JumpToNextNmeaMessage()
    {
        int c = Stream.ReadByte();
        while (c != -1 && c != '$')
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

    /// <summary>
    /// Reads the next NMEA message from the underlying stream.
    /// </summary>
    /// <returns>A string containing the next NMEA message, including the terminating newline character.</returns>
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

    /// <summary>
    /// Checks whether the next NMEA message in the stream is a GGA message.
    /// </summary>
    /// <returns><see langword="true"/> if the next NMEA message is a GGA message; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Retrieves the next sequence of NMEA messages up to, but not including, the next GGA message.
    /// </summary>
    /// <returns>An array of strings containing the NMEA messages read in sequence until a GGA message is encountered.</returns>
    private string[] ConsumeNextChunk()
    {
        List<string> chunk = [ReadNextNmeaMessage()];
        while (!PeekGGA())
        {
            chunk.Add(ReadNextNmeaMessage());
        }

        return [.. chunk];
    }

    public void Start(Dictionary<string, string>? ntripOptions = null)
    {
        Stream.Seek(0, SeekOrigin.Begin);
        NmeaTimer = new((state) =>
        {
            string[] nmeaMessages = ConsumeNextChunk();
            foreach (var message in nmeaMessages)
            {
                OnNewGPSData?.Invoke(this, message);
            }
        }, null, 0, 1000);
    }

    public void Stop()
    {
        NmeaTimer?.Dispose();
        NmeaTimer = null;
    }
}
