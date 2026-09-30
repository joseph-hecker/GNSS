using GNSS.Nmea.Exceptions;

namespace GNSS.Nmea;

public class VTG : NmeaSentence
{
    public double CourseTrue { get; }
    public string CourseTrueUnits { get; }
    public double CourseMagnetic { get; }
    public string CourseMagneticUnits { get; }
    public double SpeedKnots { get; }
    public string SpeedKnotsUnits { get; }
    public double SpeedKMH { get; }
    public string SpeedKMHUnits { get; }
    public PositionMode PositionMode { get; }

    public VTG(string message) : base(message)
    {
        try
        {
            CourseTrue = double.Parse(_parts[1]);
            CourseTrueUnits = _parts[2];
            CourseMagnetic = double.Parse(_parts[3]);
            CourseMagneticUnits = _parts[4];
            SpeedKnots = double.Parse(_parts[5]);
            SpeedKnotsUnits = _parts[6];
            SpeedKMH = double.Parse(_parts[7]);
            SpeedKMHUnits = _parts[8];
            PositionMode = ParsePositionMode(_parts[9]);
        }
        catch (Exception ex)
        {
            throw new NmeaException($"Error parsing VTG message. ({_message})", ex);
        }
    }

    public override void UpdateGPSData(GPSData data)
    {
        data.Course = CourseTrue;
        data.Speed = SpeedKnots;
        data.PositionMode = PositionMode;
    }
}
