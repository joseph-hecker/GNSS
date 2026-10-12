using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public abstract class NmeaSentence : ILocationInfo, IEquatable<NmeaSentence>
{
    protected readonly string _message;
    protected readonly string[] _parts;

    public NmeaType ID { get; }

    public NmeaSentence(string message)
    {
        try
        {
            _message = message.Replace("\r\n", "");
            _parts = message[1..^3].Split(',');
            ID = Enum.Parse<NmeaType>(_parts[0]);
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing NMEA message.", ex, message);
        }
    }

    public abstract void UpdateGPSData(GPSData data);

    public void ValidateChecksum()
    {
        int index = _message.IndexOf('*');
        byte expected = Convert.ToByte(_message[(index + 1)..], 16);
        byte checksum = 0;
        for (int i = 1; i < index; i++)
        {
            checksum ^= Convert.ToByte(_message[i]);
        }

        if (checksum != expected)
        {
            throw new ChecksumException($"Expected checksum 0x{expected:X} ({expected}), but got 0x{checksum:X} ({checksum}).", _message);
        }
    }

    public bool Equals(NmeaSentence? other)
    {
        return _message.Equals(other?._message);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as NmeaSentence);
    }

    public override int GetHashCode()
    {
        return _message.GetHashCode();
    }

    public override string ToString()
    {
        return _message;
    }

    public static NmeaSentence? Parse(string message)
    {
        try
        {
            int index = message.IndexOf(',');
            _ = Enum.TryParse(message[1..index], out NmeaType nmeaType);
            NmeaSentence? sentence = nmeaType switch
            {
                NmeaType.GGA => new GGA(message),
                NmeaType.GLL => new GLL(message),
                NmeaType.GSA => new GSA(message),
                NmeaType.GST => new GST(message),
                NmeaType.GSV => new GSV(message),
                NmeaType.RMC => new RMC(message),
                NmeaType.RME => new RME(message),
                NmeaType.VTG => new VTG(message),
                NmeaType.ENAV => new ENAV(message),

                _ => null,
            };

            sentence?.ValidateChecksum();
            return sentence;
        }
        catch (NmeaException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing NMEA message.", ex, message);
        }
    }

    protected static TimeOnly ParseTime(string time)
    {
        return TimeOnly.ParseExact(time, "HHmmss.ff");
    }

    protected static DateOnly ParseDate(string date)
    {
        return DateOnly.ParseExact(date, "ddMMyy");
    }

    protected static PositionMode ParsePositionMode(string mode)
    {
        return mode switch
        {
            "A" => PositionMode.Autonomous,
            "D" => PositionMode.Differential,
            "E" => PositionMode.Estimated,
            "M" => PositionMode.Manual,
            "S" => PositionMode.Simulated,
            "N" or _ => PositionMode.Invalid,
        };
    }
}
