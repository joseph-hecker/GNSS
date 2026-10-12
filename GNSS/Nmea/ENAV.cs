using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class ENAV : NmeaSentence, ILocation
{
    public TimeOnly Time { get; }
    public double CompLat { get; }
    public double CompLon { get; }
    public double RawLat { get; }
    public double RawLon { get; }
    public double RawAlt { get; }
    public double CompAlt { get; }
    public double Roll { get; }
    public double Pitch { get; }
    public double Yaw { get; }
    public double TiltAngle { get; }
    public double PositionQuality { get; }
    public FixQuality RawFix { get; }
    public int AntennaHeight { get; }
    public int LidarStrength { get; }
    public EnavMode EnavMode { get; }
    public SmartHandleStatus SHStatus { get; }

    /// <summary>
    /// If the Skadi's tilt compensation has been initialized. Requires <see cref="FixQuality.RTKFixed"/> and IMU calibration.
    /// </summary>
    public bool TiltCompInitilized => PositionQuality == -1;

    /// <summary>
    /// If the Skadi's trigger is being pressed. Only applies for extensible pole mode.
    /// </summary>
    public bool TriggerIsPressed => EnavMode == EnavMode.ExtensiblePoleTriggerPressed;

    /// <summary>
    /// The height difference between the Skadi and the ground.
    /// </summary>
    public double InvisiblePoleHeight => RawAlt - CompAlt;

    public ENAV(string message) : base(message)
    {
        try
        {
            Time = ParseTime(_parts[1]);
            CompLat = double.Parse(_parts[2]);
            CompLon = double.Parse(_parts[3]);
            RawLat = double.Parse(_parts[4]);
            RawLon = double.Parse(_parts[5]);
            RawAlt = double.Parse(_parts[6]);
            CompAlt = double.Parse(_parts[7]);
            Roll = double.Parse(_parts[8]);
            Pitch = double.Parse(_parts[9]);
            Yaw = double.Parse(_parts[10]);
            PositionQuality = double.Parse(_parts[11]);
            TiltAngle = double.Parse(_parts[13]);
            RawFix = Enum.Parse<FixQuality>(_parts[17]);
            AntennaHeight = int.Parse(_parts[20]);
            LidarStrength = int.Parse(_parts[21]);
            EnavMode = Enum.Parse<EnavMode>(_parts[22]);
            SHStatus = Enum.Parse<SmartHandleStatus>(_parts[23]);
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing ENAV message.", ex, message);
        }
    }

    public double Latitude()
    {
        return TiltCompInitilized ? CompLat : RawLat;
    }

    public double Longitude()
    {
        return TiltCompInitilized ? CompLon : RawLon;
    }

    public double Altitude()
    {
        return TiltCompInitilized ? CompAlt : RawAlt;
    }

    public override void UpdateGPSData(GPSData data)
    {
        data.UTCTime = Time;
        data.Latitude = Latitude();
        data.Longitude = Longitude();
        data.Altitude = Altitude();
        data.FixQuality = RawFix;
        data.EstimatedAccuracy = PositionQuality;
        data.EnavData = this;
    }
}
