using System.Text;

namespace NmeaService;

public class FileBuffer(string filePath)
{
    private FileStream Stream { get; } = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

    private string ReadLine()
    {
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

        if (c == -1 && sb.Length == 0)
        {
            Stream.Seek(0, SeekOrigin.Begin);
            return ReadLine();
        }

        return sb.ToString();
    }

    private bool PeekGGA()
    {
        StringBuilder sb = new();
        long start = Stream.Position;
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

    public string[] ComsumeNextChunk()
    {
        List<string> chunk = [ReadLine()];
        while (!PeekGGA())
        {
            chunk.Add(ReadLine());
        }

        return [..chunk];
    }
}
